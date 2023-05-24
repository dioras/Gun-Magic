using _GAME.Level;
using System;
using UnityEngine;

namespace _GAME.RateUs
{
    public class RateUsShowLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private RateUsFeature _rateUsFeature;

        private const string appUrl = "https://play.google.com/store/apps/details?id=portal.hero";

        private int playerRating = 0;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature; //GameFeature._levelFeature;
            _rateUsFeature = GameFeature.RateUsFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelComplete += ShowRateUs;
            _rateUsFeature.RateUsViewRefs.BtnClose.onClick.AddListener(OnBtnCloseClicked);
            _rateUsFeature.RateUsViewRefs.BtnOk.onClick.AddListener(OnBtnOkClicked);

            foreach (var btnRefs in _rateUsFeature.RateUsViewRefs.BtnStars)
                btnRefs.Btn.onClick.AddListener(() => OnStarClicked(btnRefs));
        }

        private void ShowRateUs()
        {
            if (_rateUsFeature.RateUsData.IsGameRated)
                return;

            if (_levelFeature.CurrentLevel < 2)
                return;

            var ticksSinceLastShow = DateTime.Now.Ticks - _rateUsFeature.RateUsData.LastRateUsShowTime;
            var hoursSinceLastShow = TimeSpan.FromTicks(ticksSinceLastShow).TotalHours;

            if (hoursSinceLastShow < 24)
                return;

            _rateUsFeature.RateUsData.LastRateUsShowTime = DateTime.Now.Ticks;
            _rateUsFeature.OnSave?.Invoke();

            _rateUsFeature.RateUsViewRefs.BtnOk.interactable = false;
            _rateUsFeature.RateUsViewRefs.View.Show();
        }

        private void OnStarClicked(RateUsBtnRefs btnRefs)
        {
            playerRating = btnRefs.StarCount;

            for (int i = 0; i < _rateUsFeature.RateUsViewRefs.BtnStars.Length; i++)
            {
                bool showStar = i < playerRating;

                var clickedBtnTransform = _rateUsFeature.RateUsViewRefs.BtnStars[i].transform;
                clickedBtnTransform.GetChild(0).gameObject.SetActive(showStar);
            }

            _rateUsFeature.RateUsViewRefs.BtnOk.interactable = true;
        }

        private void OnBtnOkClicked()
        {
            _rateUsFeature.RateUsData.IsGameRated = true;
            _rateUsFeature.OnSave?.Invoke();

            if (playerRating == 5)
                Application.OpenURL(appUrl);

            _rateUsFeature.RateUsViewRefs.View.Hide();
        }

        private void OnBtnCloseClicked()
        {
            _rateUsFeature.RateUsData.LastRateUsShowTime = DateTime.Now.Ticks;

            _rateUsFeature.OnSave?.Invoke();

            _rateUsFeature.RateUsViewRefs.View.Hide();
        }
    }
}
