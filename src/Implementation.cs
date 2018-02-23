using UnityEngine;

namespace CoordinatesGrabber
{
    public class Implementation
    {
        public static void OnLoad()
        {
            Debug.Log("[Coordinates-Grabber]: Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }
    }
}