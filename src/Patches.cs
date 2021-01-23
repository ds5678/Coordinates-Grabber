using Harmony;
using UnityEngine;

namespace CoordinatesGrabber
{
    internal class FormatHelper
    {
        internal static string FormatVector(Vector3 vector)
        {
            return string.Format("{0:F4},{1:F4},{2:F4}", vector.x, vector.y, vector.z);
        }
    }

    internal class KeyTracker
    {
        internal static bool showName = false;
        internal static bool showPosition = false;
        internal static bool showRotation = false;
        internal static bool showScene = false;
        internal static bool showLootTable = false;
    }

    [HarmonyPatch(typeof(GameManager),"Update")]
    internal class CheckForKeyPresses
    {
        private static void Postfix()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.N))
            {
                KeyTracker.showName = !KeyTracker.showName;
                KeyTracker.showPosition = false;
                KeyTracker.showRotation = false;
                KeyTracker.showScene = false;
                KeyTracker.showLootTable = false;
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.P))
            {
                KeyTracker.showName = false;
                KeyTracker.showPosition = !KeyTracker.showPosition;
                KeyTracker.showRotation = false;
                KeyTracker.showScene = false;
                KeyTracker.showLootTable = false;
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.R))
            {
                KeyTracker.showName = false;
                KeyTracker.showPosition = false;
                KeyTracker.showRotation = !KeyTracker.showRotation;
                KeyTracker.showScene = false;
                KeyTracker.showLootTable = false;
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.K))
            {
                KeyTracker.showName = false;
                KeyTracker.showPosition = false;
                KeyTracker.showRotation = false;
                KeyTracker.showScene = !KeyTracker.showScene;
                KeyTracker.showLootTable = false;
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
            {
                KeyTracker.showName = false;
                KeyTracker.showPosition = false;
                KeyTracker.showRotation = false;
                KeyTracker.showScene = false;
                KeyTracker.showLootTable = !KeyTracker.showLootTable;
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), "ProcessInput")]
    internal class InputManager_ProcessInput
    {
        public static void Postfix()
        {
            if (!InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.RightControl) )
            {
                return;
            }

            if (KeyTracker.showName || KeyTracker.showPosition || KeyTracker.showRotation)
            {
                GameObject gameObject = GameManager.GetPlayerManagerComponent().m_InteractiveObjectNearCrosshair;
                if (gameObject == null)
                {
                    return;
                }

                var line = "item=" + gameObject.name + " p=" + FormatHelper.FormatVector(gameObject.transform.position) + " r=" + FormatHelper.FormatVector(gameObject.transform.rotation.eulerAngles) + " c=100";
                Implementation.Log(line);
                CopyToClipboard(line, "Item Definition");
                return;
            }

            if (KeyTracker.showScene)
            {
                var line = "scene=" + GameManager.m_ActiveScene;
                Implementation.Log(line);
                CopyToClipboard(line, "Scene Definition");
                return;
            }

            if (KeyTracker.showLootTable)
            {
                GameObject gameObject = GameManager.GetPlayerManagerComponent().m_InteractiveObjectNearCrosshair;
                if (gameObject == null)
                {
                    return;
                }

                Container container = gameObject.GetComponentInChildren<Container>();
                if (container == null)
                {
                    return;
                }

                var line = "loottable=" + LootTableHelper.GetLootTableName(container);
                Implementation.Log(line);
                CopyToClipboard(line, "LootTable Definition");
                return;
            }
        }

        private static void CopyToClipboard(string line, string informationType)
        {
            GUIUtility.systemCopyBuffer = line;

            HUDMessage.AddMessage(informationType + " copied to clipboard");
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
            if (KeyTracker.showName)
            {
                __result += "\nname = " + __instance.m_InteractiveObjectUnderCrosshair.name;
                return;
            }

            if (KeyTracker.showPosition)
            {
                __result += "\nposition = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.position);
                return;
            }

            if (KeyTracker.showRotation)
            {
                __result += "\nrotation = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.rotation.eulerAngles);
                return;
            }

            if (KeyTracker.showScene)
            {
                __result += "\nscene = " + GameManager.m_ActiveScene;
                return;
            }

            if (KeyTracker.showLootTable)
            {
                Container container = __instance.m_InteractiveObjectUnderCrosshair.GetComponentInChildren<Container>();
                if (container != null)
                {
                    __result += "\nloottable = " + LootTableHelper.GetLootTableName(container);
                    return;
                }
            }
        }
    }
}