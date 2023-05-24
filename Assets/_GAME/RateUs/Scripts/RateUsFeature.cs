using System;
using UnityEngine;

namespace _GAME.RateUs
{
    public class RateUsFeature : MonoBehaviour
    {
        public Action<RateUsData> OnLoadData;
        public Func<RateUsData> OnSaveData;
        public Action OnSave;

        public RateUsData RateUsData;

        public RateUsViewRefs RateUsViewRefs;
    }
}