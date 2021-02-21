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
    internal enum KeyCodeAlphabet
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }

    internal class Settings : JsonModSettings
    {
        [Name("Use Middle Mouse Button")]
        [Description("Setting to false disables the middle mouse button functionality.")]
        public bool useMiddleMouseButton = true;

        [Name("Use Key Presses")]
        [Description("Setting to false disables the key press functionality.")]
        public bool useKeyPresses = true;

        [Name("Name Key")]
        [Description("The key you press to toggle name mode.")]
        public KeyCodeAlphabet nameKey = KeyCodeAlphabet.N;

        [Name("Position Key")]
        [Description("The key you press to toggle position mode.")]
        public KeyCodeAlphabet positionKey = KeyCodeAlphabet.P;

        [Name("Rotation Key")]
        [Description("The key you press to toggle rotation mode.")]
        public KeyCodeAlphabet rotationKey = KeyCodeAlphabet.R;

        [Name("Scene Key")]
        [Description("The key you press to toggle scene mode.")]
        public KeyCodeAlphabet sceneKey = KeyCodeAlphabet.K;

        [Name("Loot Table Key")]
        [Description("The key you press to toggle loot table mode.")]
        public KeyCodeAlphabet lootTableKey = KeyCodeAlphabet.L;

        [Name("Enable Delete Function")]
        [Description("Pressing the Delete key will delete the interactible object under your crosshair.")]
        public bool enableDelete = false;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == nameof(useKeyPresses))
            {
                GrabberSettings.SetFieldVisible((bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();
            UpdateKeySettings();
        }

        internal void UpdateKeySettings()
        {
            GrabberSettings.nameKey = (KeyCode)Enum.Parse(typeof(KeyCode), nameKey.ToString());
            GrabberSettings.positionKey = (KeyCode)Enum.Parse(typeof(KeyCode), positionKey.ToString());
            GrabberSettings.rotationKey = (KeyCode)Enum.Parse(typeof(KeyCode), rotationKey.ToString());
            GrabberSettings.sceneKey = (KeyCode)Enum.Parse(typeof(KeyCode), sceneKey.ToString());
            GrabberSettings.lootTableKey = (KeyCode)Enum.Parse(typeof(KeyCode), lootTableKey.ToString());
        }
    }

    internal static class GrabberSettings
    {
        internal static readonly Settings settings = new Settings();
        internal static KeyCode nameKey;
        internal static KeyCode positionKey;
        internal static KeyCode rotationKey;
        internal static KeyCode sceneKey;
        internal static KeyCode lootTableKey;

        public static void OnLoad()
        {
            settings.AddToModSettings("Coordinate Grabber");
            settings.UpdateKeySettings();
        }
        internal static void SetFieldVisible(bool visible)
        {
            FieldInfo[] fields = settings.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; ++i)
            {
                string[] keyFieldNames = new string[]
                {
                    nameof(settings.nameKey),
                    nameof(settings.positionKey),
                    nameof(settings.rotationKey),
                    nameof(settings.sceneKey),
                    nameof(settings.lootTableKey)
                };
                if (keyFieldNames.Contains<string>(fields[i].Name))
                {
                    settings.SetFieldVisible(fields[i], visible);
                }
            }
        }
    }
}
