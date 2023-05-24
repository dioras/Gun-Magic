using System;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.LevelUIView
{
    public class LevelUIFeature : MonoBehaviour
    {
        public UIRefs LevelCompleteView;
        public UIRefs LevelFailedView;
        public UIRefs TapToStartView;
        public WeaponsHubViewRefs WeaponsHubView;
        public UIRefs GamePlayView;
        public LoadingScreenRefs LoadingScreenView;
        public LevelCompleteViewRefs LevelCompleteViewRefs;
        public GamePlayRefs GamePlayScreenView;
        public Image WhiteOverlay;

        public Button ButtonNext;
        public Button SkipLevelButton;
        public CanvasGroup SkipLevelCanvasGroup;
        public TMPro.TMP_Text NoAdtext;

        public Action<int> OnCompleteLevel;
        public Action<int> OnFailedLevel;
        public Action OnTapToStart;
        public Action OnLevelLoaded;

        public Action OnReloadFailedLevel;
        public Action OnClickNextLevel;
        public Action OnShowLoadingScreen;
        public Action OnShowOnlyLoadingScreen;
        public Action OnHideLoadingScreen;
        public Action OnHideTapToStart;
        public Action OnShowTapToStart;

        public Action OnShowSkipButton;
        public Action<bool> OnHideSkipButton;

        public Action OnRestartlevelButtonCkick;
        public Action OnSkipLevelButtonClick;

        public Action<float> OnUpdateProgressBar;
        public Action<string> OnUpdateStartTimer;

        public Action OnUpdatePreogreesBarView;

        public float DelayToShowCompleteView = 1;
    }
}
