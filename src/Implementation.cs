using UnityEngine;
using MelonLoader;
using Rewired;
using System.IO;

namespace CoordinatesGrabber
{
    public class Implementation : MelonMod
    {
        public const string NAME = "Coordinates-Grabber";

        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
            GrabberSettings.OnLoad();
        }

        internal static void Log(string message)
        {
            MelonLogger.Log( message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format(message, parameters);
            Log(preformattedMessage);
        }

        internal static string GetModsFolderPath()
        {
            return Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods");
        }
    }
}