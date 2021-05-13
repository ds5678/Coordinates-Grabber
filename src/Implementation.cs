﻿using MelonLoader;
using System.IO;
using UnityEngine;

namespace CoordinatesGrabber
{
	public static class BuildInfo
	{
		public const string Name = "Coordinates-Grabber"; // Name of the Mod.  (MUST BE SET)
		public const string Description = "A mod for getting in game coordinates of items."; // Description for the Mod.  (Set as null if none)
		public const string Author = "Wulf Marius, ds5678"; // Author of the Mod.  (MUST BE SET)
		public const string Company = null; // Company that made the Mod.  (Set as null if none)
		public const string Version = "4.2.1"; // Version of the Mod.  (MUST BE SET)
		public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
	}
	public class Implementation : MelonMod
	{
		public override void OnApplicationStart()
		{
			Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
			Settings.OnLoad();
		}

		internal static void Log(string message, params object[] parameters) => MelonLogger.Log(message, parameters);

		internal static string GetModsFolderPath() => Path.GetFullPath(typeof(MelonMod).Assembly.Location + @"\..\..\Mods");
	}
}