using UnityEngine;

namespace _GAME.VibroService
{
    public class VibroFeature : MonoBehaviour
    {
        public bool IsVibroOn = true;

        public System.Action OnVibrateLevelComplete;
        public System.Action OnVibrateLevelFailed;
        public System.Action OnVibrateLight;
        public System.Action OnVibrateMedium;
        public System.Action OnVibrateHard;

        public System.Action OnSave;

        public IconSwitchRefs Icon;
    }
}
