using HarmonyLib;
using Il2Cpp;
using MelonLoader.Utils;
using UnityEngine;
using CustomInput = KeyboardUtilities.InputManager;

namespace CoordinatesGrabber
{
	internal class FormatHelper
	{
		internal static string FormatVector(Vector3 vector)
		{
			string x = vector.x.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
			string y = vector.y.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
			string z = vector.z.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
			return string.Format("{0},{1},{2}", x, y, z);
		}
		internal static string FormatVector(Vector2 vector)
		{
			string x = vector.x.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
			string y = vector.y.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
			return string.Format("{0},{1}", x, y);
		}
	}

	internal enum GrabberMode
	{
		None,
		Scene,
		Name,
		Position,
		Rotation,
		LootTable
	}

	[HarmonyPatch]
	internal class KeyTracker
	{
		//		internal static bool useKeyPresses { get; set; } = false;
		internal static GrabberMode currentMode { get; set; } = GrabberMode.None;
		internal static GameObject? lookingAt { get; set; } = null;

		internal static UILabel hudLabel = new();

		internal static void ApplyKeyPress(GrabberMode modeAssociatedWithKey)
		{
			if (currentMode == modeAssociatedWithKey)
			{
				currentMode = GrabberMode.None;
			}
			else
			{
				HUDMessage.AddMessage("Mode: "+ modeAssociatedWithKey.ToString(),0.5f);
				currentMode = modeAssociatedWithKey;
				ResetLooatingAt();
			}
		}

		internal static int Modulo(int initialValue)
		{
			if (initialValue < 0) return Modulo(initialValue + 6);
			else if (initialValue >= 6) return Modulo(initialValue - 6);
			else return initialValue;
		}

		internal static void ApplyScroll()
		{
			if (Settings.options.useMiddleMouseButton && !GameManager.GetPlayerManagerComponent().IsInPlacementMode())
			{
				int delta = (int)CustomInput.GetMouseScrollDelta()[1];
				int currentPosition = (int)currentMode;
				int newPosition = currentPosition - delta;
				if (delta != 0)
				{
					ApplyKeyPress((GrabberMode)Modulo(newPosition));
				}
			}
		}

		internal static void ResetLooatingAt()
		{
			hudLabel.enabled = false;
			hudLabel.text = "";
			lookingAt = null;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Panel_HUD), nameof(Panel_HUD.Initialize))]
		public static void Panel_HUD_Initialize_Postfix(Panel_HUD __instance)
		{
			hudLabel = UILabel.Instantiate<UILabel>(__instance.m_Label_SurvivalTime, __instance.m_Label_SurvivalTime.transform.parent);
			hudLabel.capsLock = false;
			hudLabel.name = "CoordGrabberLabel";

		}
	}

	[HarmonyPatch(typeof(GameManager), nameof(GameManager.Update))]
	internal class CheckForKeyPresses
	{
		private static void Postfix()
		{
			if (Settings.options.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.nameKey))
			{
				KeyTracker.ApplyKeyPress(GrabberMode.Name);
			}
			if (Settings.options.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.positionKey))
			{
				KeyTracker.ApplyKeyPress(GrabberMode.Position);
			}
			if (Settings.options.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.rotationKey))
			{
				KeyTracker.ApplyKeyPress(GrabberMode.Rotation);
			}
			if (Settings.options.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.sceneKey))
			{
				KeyTracker.ApplyKeyPress(GrabberMode.Scene);
			}
			if (Settings.options.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.lootTableKey))
			{
				KeyTracker.ApplyKeyPress(GrabberMode.LootTable);
			}
			KeyTracker.ApplyScroll();
			if (!Settings.options.useKeyPresses && !Settings.options.useMiddleMouseButton && KeyTracker.currentMode != GrabberMode.None)
			{
				KeyTracker.ApplyKeyPress(GrabberMode.None);
			}
		}
	}

	[HarmonyPatch(typeof(InputManager), nameof(InputManager.ProcessInput))]
	internal class InputManager_ProcessInput
	{
		public static void Postfix()
		{
			bool controlDown = InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.RightControl);
			bool middleMouseDown = Settings.options.useMiddleMouseButton && CustomInput.GetMouseButtonDown(2);
			bool altDown = Settings.options.useKeyPresses && (CustomInput.GetKeyDown(KeyCode.LeftAlt) || CustomInput.GetKeyDown(KeyCode.RightAlt));
			bool saveToFile = middleMouseDown || altDown;

			if ((!controlDown && !saveToFile) || InterfaceManager.IsOverlayActiveImmediate()) return;

			SaveInformation(saveToFile);
		}
		private static void SaveInformation(bool saveToFile)
		{
			float pickupRange = GameManager.GetGlobalParameters().m_MaxPickupRange;

			string line;
			switch (KeyTracker.currentMode)
			{
				case GrabberMode.None:
					return;
				case GrabberMode.Scene:
					line = "scene=" + GameManager.m_ActiveScene;
					RecordData(line, "Scene Definition", saveToFile);
					return;
				case GrabberMode.LootTable:
					GameObject? gameObject1 = GameManager.GetPlayerManagerComponent().GetInteractiveObjectUnderCrosshairs(pickupRange);

					if (gameObject1 == null) return;

					Container container = gameObject1.GetComponentInChildren<Container>();
					if (container == null) return;

					line = "loottable=" + LootTableHelper.GetLootTableName(container);
					RecordData(line, "LootTable Definition", saveToFile);
					return;
				default: //Name, Position, or Rotation
					GameObject? gameObject2 = GameManager.GetPlayerManagerComponent().GetInteractiveObjectUnderCrosshairs(pickupRange);
					if (gameObject2 == null) return;

					line = "item=" + gameObject2.name + " p=" + FormatHelper.FormatVector(gameObject2.transform.position) + " r=" + FormatHelper.FormatVector(gameObject2.transform.rotation.eulerAngles) + " c=100";
					RecordData(line, "Item Definition", saveToFile);
					return;
			}
		}
		private static void CopyToClipboard(string line, string informationType)
		{
			GUIUtility.systemCopyBuffer = line;
			HUDMessage.AddMessage(informationType + " copied to clipboard");
		}
		private static void AppendToFile(string line, string informationType)
		{
			StreamWriter file = File.AppendText(Path.Combine(MelonEnvironment.ModsDirectory, @"Coordinates-Grabber-Output.txt"));
			file.WriteLine(line);
			file.Close();
			HUDMessage.AddMessage(informationType + " appended to file");
		}
		private static void RecordData(string line, string informationType, bool append)
		{
			if (append) AppendToFile(line, informationType);
			else CopyToClipboard(line, informationType);
		}
	}

	internal class LootTableHelper
	{
		internal static string? GetLootTableName(Container? container)
		{
			if (container == null) return null;

			if (container.IsLocked() && container.m_LockedLootTableData != null) return container.m_LockedLootTableData.name;

			if (container.m_LootTableData != null) return container.m_LootTableData.name;

			return null;
		}
	}

	[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.GetInteractiveObjectUnderCrosshairs), new Type[] { typeof(float) })]
	internal class GetInteractiveObjectUnderCrosshairs
	{
		public static void Postfix(ref GameObject? __result)
		{
			if (__result == null || string.IsNullOrWhiteSpace(GameManager.m_ActiveScene))
			{
				KeyTracker.ResetLooatingAt();
				return;
			}

			if (Settings.options.useDeleteFunction && CustomInput.GetKeyDown(Settings.options.deleteKey))
			{
				UnityEngine.Object.Destroy(__result);
				KeyTracker.ResetLooatingAt();
				__result = null;
				return;
			}

			if (KeyTracker.lookingAt == __result || KeyTracker.currentMode == GrabberMode.None)
			{
				return;
			}

			string outputMsg = string.Empty;

			switch (KeyTracker.currentMode)
			{
				case GrabberMode.Name:
					outputMsg = "\nname = " + __result.name;
					break;
				case GrabberMode.Position:
					outputMsg = "\nposition = " + FormatHelper.FormatVector(__result.transform.position);
					break;
				case GrabberMode.Rotation:
					outputMsg = "\nrotation = " + FormatHelper.FormatVector(__result.transform.rotation.eulerAngles);
					break;
				case GrabberMode.Scene:
					outputMsg = "\nscene = " + GameManager.m_ActiveScene;
					break;
				case GrabberMode.LootTable:
					Container container = __result.GetComponentInChildren<Container>();
					if (container != null)
					{
						outputMsg = "\nloottable = " + LootTableHelper.GetLootTableName(container);
					}
					else
					{
						return;
					}
					break;
			}

			if (!string.IsNullOrWhiteSpace(outputMsg))
			{
				KeyTracker.lookingAt = __result;
				KeyTracker.hudLabel.enabled = true;
				KeyTracker.hudLabel.text = outputMsg;
			}
		}
	}

}