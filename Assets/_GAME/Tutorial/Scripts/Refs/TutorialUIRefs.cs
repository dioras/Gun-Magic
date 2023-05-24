using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Tutorial
{
    public class TutorialUIRefs : MonoBehaviour
    {
        public TutorialPart[] TutorialParts;

        public int CurrentPartIndex = 0;

        public int Step_Part;
    }

    [System.Serializable]
    public class TutorialPart
    {
        public Button[] Targets;
    }
}