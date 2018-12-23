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

    [HarmonyPatch(typeof(InputManager), "ProcessInput")]
    internal class InputManager_ProcessInput
    {
        public static void Postfix()
        {
            if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.R))
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

            if (Input.GetKeyDown(KeyCode.L))
            {
                var line = "scene=" + GameManager.m_ActiveScene;
                Implementation.Log(line);
                CopyToClipboard(line, "Scene Definition");
                return;
            }

            if (Input.GetKeyDown(KeyCode.K))
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
            if (Input.GetKey(KeyCode.P))
            {
                __result += "\nposition = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.position);
                return;
            }

            if (Input.GetKey(KeyCode.R))
            {
                __result += "\nrotation = " + FormatHelper.FormatVector(__instance.m_InteractiveObjectUnderCrosshair.transform.rotation.eulerAngles);
                return;
            }

            if (Input.GetKey(KeyCode.L))
            {
                __result += "\nscene = " + GameManager.m_ActiveScene;
                return;
            }

            if (Input.GetKey(KeyCode.K))
            {
                Container container = __instance.m_InteractiveObjectUnderCrosshair.GetComponentInChildren<Container>();
                if (container != null)
                {
                    __result += "\nloottable = " + LootTableHelper.GetLootTableName(container);
                }
                return;
            }
        }
    }
}