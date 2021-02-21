using Harmony;
using System.IO;
using UnityEngine;
using CustomInput = KeyboardUtilities.InputManager;

namespace CoordinatesGrabber
{
    internal class FormatHelper
    {
        internal static string FormatVector(Vector3 vector)
        {
            return string.Format("{0:F4},{1:F4},{2:F4}", vector.x, vector.y, vector.z);
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

    internal class KeyTracker
    {
        internal const bool useKeyPresses = false;
        internal static GrabberMode currentMode = GrabberMode.None;

        internal static void ApplyKeyPress(GrabberMode modeAssociatedWithKey)
        {
            if(currentMode == modeAssociatedWithKey)
            {
                currentMode = GrabberMode.None;
            }
            else
            {
                currentMode = modeAssociatedWithKey;
            }
        }

        internal static int Modulo(int initialValue)
        {
            if (initialValue < 0)
            {
                return Modulo(initialValue + 6);
            }
            else if(initialValue >= 6)
            {
                return Modulo(initialValue - 6);
            }
            else
            {
                return initialValue;
            }
        }

        internal static void ApplyScroll()
        {
            if (GrabberSettings.settings.useMiddleMouseButton)
            {
                int delta = (int)CustomInput.MouseScrollDelta[1];
                int currentPosition = (int)currentMode;
                int newPosition = currentPosition - delta;
                currentMode = (GrabberMode)Modulo(newPosition);
            }
        }
    }

    [HarmonyPatch(typeof(GameManager),"Update")]
    internal class CheckForKeyPresses
    {
        private static void Postfix()
        {
            if (GrabberSettings.settings.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, GrabberSettings.nameKey))
            {
                KeyTracker.ApplyKeyPress(GrabberMode.Name);
            }
            if (GrabberSettings.settings.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, GrabberSettings.positionKey))
            {
                KeyTracker.ApplyKeyPress(GrabberMode.Position);
            }
            if (GrabberSettings.settings.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, GrabberSettings.rotationKey))
            {
                KeyTracker.ApplyKeyPress(GrabberMode.Rotation);
            }
            if (GrabberSettings.settings.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, GrabberSettings.sceneKey))
            {
                KeyTracker.ApplyKeyPress(GrabberMode.Scene);
            }
            if (GrabberSettings.settings.useKeyPresses && InputManager.GetKeyDown(InputManager.m_CurrentContext, GrabberSettings.lootTableKey))
            {
                KeyTracker.ApplyKeyPress(GrabberMode.LootTable);
            }
            KeyTracker.ApplyScroll();
            if(!GrabberSettings.settings.useKeyPresses && !GrabberSettings.settings.useMiddleMouseButton && KeyTracker.currentMode != GrabberMode.None)
            {
                KeyTracker.ApplyKeyPress(GrabberMode.None);
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), "ProcessInput")]
    internal class InputManager_ProcessInput
    {
        public static void Postfix()
        {
            bool controlDown = InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.RightControl);
            bool middleMouseDown = GrabberSettings.settings.useMiddleMouseButton && CustomInput.GetMouseButtonDown(2);
            bool altDown = GrabberSettings.settings.useKeyPresses && (CustomInput.GetKeyDown(KeyCode.LeftAlt) || CustomInput.GetKeyDown(KeyCode.RightAlt));
            bool saveToFile = middleMouseDown || altDown;
            if (!controlDown && !saveToFile )
            {
                return;
            }
            SaveInformation(saveToFile);
        }

        private static void SaveInformation(bool saveToFile)
        {
            string line = "";
            switch (KeyTracker.currentMode)
            {
                case GrabberMode.None:
                    return;
                case GrabberMode.Scene:
                    line = "scene=" + GameManager.m_ActiveScene;
                    RecordData(line, "Scene Definition", saveToFile);
                    return;
                case GrabberMode.LootTable:
                    GameObject gameObject1 = GameManager.GetPlayerManagerComponent().m_InteractiveObjectNearCrosshair;
                    if (gameObject1 == null)
                    {
                        return;
                    }
                    Container container = gameObject1.GetComponentInChildren<Container>();
                    if (container == null)
                    {
                        return;
                    }
                    line = "loottable=" + LootTableHelper.GetLootTableName(container);
                    RecordData(line, "LootTable Definition", saveToFile);
                    return;
                default: //Name, Position, or Rotation
                    GameObject gameObject2 = GameManager.GetPlayerManagerComponent().m_InteractiveObjectNearCrosshair;
                    if (gameObject2 == null)
                    {
                        return;
                    }
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
            //StreamWriter file = File.AppendText(Path.Combine(Implementation.GetModsFolderPath(), @"templates\test.txt"));
            StreamWriter file = File.AppendText(Path.Combine(Implementation.GetModsFolderPath(), @"Coordinates-Grabber-Output.txt"));
            file.WriteLine(line);
            file.Close();
            HUDMessage.AddMessage(informationType + " appended to file");
        }
        private static void RecordData(string line, string informationType, bool append)
        {
            if (append)
            {
                AppendToFile(line, informationType);
            }
            else
            {
                CopyToClipboard(line, informationType);
            }
        }
    }

    internal class LootTableHelper
    {
        internal static string GetLootTableName(Container container)
        {
            if (container == null)
            {
                return null;
            }

            if (container.IsLocked() && container.m_LockedLootTablePrefab != null)
            {
                return container.m_LockedLootTablePrefab.name;
            }

            if (container.m_LootTablePrefab != null)
            {
                return container.m_LootTablePrefab.name;
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectDisplayText")]
    internal class PlayerManager_GetInteractiveObjectDisplayText
    {
        public static void Postfix(PlayerManager __instance, ref string __result)
        {
            switch (KeyTracker.currentMode)
            {
                case GrabberMode.Name:
                    __result += "\nname = " + __instance.m_InteractiveObjectUnderCrosshair.name;
                    break;
                case GrabberMode.Position:
                    __result += "\nposition = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.position);
                    break;
                case GrabberMode.Rotation:
                    __result += "\nrotation = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.rotation.eulerAngles);
                    break;
                case GrabberMode.Scene:
                    __result += "\nscene = " + GameManager.m_ActiveScene;
                    break;
                case GrabberMode.LootTable:
                    Container container = __instance.m_InteractiveObjectUnderCrosshair.GetComponentInChildren<Container>();
                    if (container != null)
                    {
                        __result += "\nloottable = " + LootTableHelper.GetLootTableName(container);
                    }
                    break;
            }
            if (GrabberSettings.settings.enableDelete && CustomInput.GetKeyDown(KeyCode.Delete))
            {
                UnityEngine.Object.Destroy(__instance.m_InteractiveObjectUnderCrosshair);
            }
        }
    }
}