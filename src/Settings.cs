using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModSettings;
using UnityEngine;

namespace CoordinatesGrabber
{
    internal class GrabberSettings : JsonModSettings
    {
        [Name("Use Middle Mouse Button")]
        [Description("Setting to false disables the middle mouse button functionality.")]
        public bool useMiddleMouseButton = true;

        [Name("Use Key Presses")]
        [Description("Setting to false disables the key press functionality.")]
        public bool useKeyPresses = true;

        [Name("Name Key")]
        [Description("The key you press to toggle name mode.")]
        public KeyCode nameKey = KeyCode.N;

        [Name("Position Key")]
        [Description("The key you press to toggle position mode.")]
        public KeyCode positionKey = KeyCode.P;

        [Name("Rotation Key")]
        [Description("The key you press to toggle rotation mode.")]
        public KeyCode rotationKey = KeyCode.R;

        [Name("Scene Key")]
        [Description("The key you press to toggle scene mode.")]
        public KeyCode sceneKey = KeyCode.K;

        [Name("Loot Table Key")]
        [Description("The key you press to toggle loot table mode.")]
        public KeyCode lootTableKey = KeyCode.L;

        [Name("Enable Delete Function")]
        [Description("Pressing the Delete key will delete the interactible object under your crosshair.")]
        public bool enableDelete = false;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == nameof(useKeyPresses))
            {
                Settings.SetFieldVisible((bool)newValue);
            }
        }
    }

    internal static class Settings
    {
        internal static GrabberSettings options;

        public static void OnLoad()
        {
            options = new GrabberSettings();
            options.AddToModSettings("Coordinate Grabber");
        }
        internal static void SetFieldVisible(bool visible)
        {
            FieldInfo[] fields = options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; ++i)
            {
                string[] keyFieldNames = new string[]
                {
                    nameof(options.nameKey),
                    nameof(options.positionKey),
                    nameof(options.rotationKey),
                    nameof(options.sceneKey),
                    nameof(options.lootTableKey)
                };
                if (keyFieldNames.Contains<string>(fields[i].Name))
                {
                    options.SetFieldVisible(fields[i], visible);
                }
            }
        }
    }
}
