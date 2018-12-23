using UnityEngine;

namespace CoordinatesGrabber
{
    public class Implementation
    {
        public const string NAME = "Coordinates-Grabber";

        public static void OnLoad()
        {
            Log("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }

        internal static void Log(string message)
        {
            Debug.LogFormat("[" + NAME + "] {0}", message);
        }
    }
}