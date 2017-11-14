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
                Debug.Log(line);
                CopyToClipboard(line, "Item definition");
                return;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                var line = "scene=" + GameManager.m_ActiveScene;
                Debug.Log(line);
                CopyToClipboard(line, "Scene definition");
                return;
            }
        }

        private static void CopyToClipboard(string line, string informationType)
        {
            TextEditor editor = new TextEditor();
            editor.text = line;
            editor.SelectAll();
            editor.Copy();

            HUDMessage.AddMessage(informationType + " copied to clipboard");
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
        }
    }
}