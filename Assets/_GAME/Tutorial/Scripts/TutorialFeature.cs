using System;
using UnityEngine;

namespace _GAME.Tutorial
{
    public class TutorialFeature : MonoBehaviour
    {
        public TutorialUIRefs[] Tutorials;

        public int CurrentTutrialIndex = 0;

        public Action<int> OnShowBattleTutorial;
    }
}
