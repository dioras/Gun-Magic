using UnityEngine;

namespace _GAME.RateUs
{
    public class SaveLoadRateUsDataLogic : MonoBehaviour
    {
        private RateUsFeature _rateUsFeature;

        private void Awake()
        {
            _rateUsFeature = GameFeature.RateUsFeature;
        }

        private void OnEnable()
        {
            _rateUsFeature.OnLoadData += LoadData;
            _rateUsFeature.OnSaveData += SaveData;
        }

        private void LoadData(RateUsData data)
        {
            if(data == null) data = new RateUsData();

            _rateUsFeature.RateUsData = data;
        }

        private RateUsData SaveData()
        {
            var data = _rateUsFeature.RateUsData;

            if (data == null) data = new RateUsData();

            return data;
        }
    }
}