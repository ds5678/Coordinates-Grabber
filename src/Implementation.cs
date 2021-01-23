using UnityEngine;
using MelonLoader;

namespace CoordinatesGrabber
{
    public class Implementation : MelonMod
    {
        public const string NAME = "Coordinates-Grabber";

        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
        }

        internal static void Log(string message)
        {
            Debug.Log(string.Format("[" + NAME + "] {0}", message));
        }
    }
}