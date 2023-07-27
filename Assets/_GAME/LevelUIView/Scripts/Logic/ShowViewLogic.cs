using _GAME.Level;
using _GAME.Shop;
using DG.Tweening;
using UnityEngine;

namespace _GAME.LevelUIView
{
    public class ShowViewLogic : MonoBehaviour
    {
        private LevelUIFeature _levelUIFeature;
        private LevelFeature _levelFeature;
        private VibroService.VibroFeature _vibroFeature;

        private void Awake()
        {
            _levelUIFeature = GetComponent<LevelUIFeature>();
            _levelFeature = GameFeature.LevelFeature;
            _vibroFeature = GameFeature.VibroFeature;
        }

        private void OnEnable()
        {
            _levelUIFeature.OnCompleteLevel += TryShowCompleteView;
            _levelUIFeature.OnCompleteLevel += (id) => HideGamePlayView();

            _levelUIFeature.OnFailedLevel += ShowFailedView;
            _levelUIFeature.OnFailedLevel += (id) => HideGamePlayView();

            _levelFeature.OnStartLevel += ShowGamePlayView;

            _levelUIFeature.OnLevelLoaded += HideLoadingScreen;
            _levelUIFeature.OnLevelLoaded += HideGamePlayView;

            _levelUIFeature.OnReloadFailedLevel += HideFailedView;

            _levelUIFeature.OnClickNextLevel += () => _levelUIFeature.OnShowLoadingScreen?.Invoke();

            _levelUIFeature.OnShowLoadingScreen += ShowLoadingScreen;
            _levelUIFeature.OnShowLoadingScreen += HideCompleteView;
            _levelUIFeature.OnShowLoadingScreen += HideFailedView;
            _levelUIFeature.OnShowLoadingScreen += HideGamePlayView;

            _levelUIFeature.OnShowOnlyLoadingScreen += ShowLoadingScreenWithoutProgressBar;

            _levelUIFeature.OnHideLoadingScreen += HideLoadingScreen;

            _levelUIFeature.OnUpdateProgressBar += UpdateProgreesBar;

            _levelUIFeature.OnUpdateStartTimer += UpdateStartTimer;

            _levelUIFeature.OnHideSkipButton += HidewSkipButton;
            _levelUIFeature.OnShowSkipButton += ShowSkipButton;

            _levelUIFeature.OnUpdatePreogreesBarView += SwitchLoadingScreenColors;

            _levelUIFeature.GamePlayScreenView.ButtonRestart.onClick.AddListener(() => _levelUIFeature.OnRestartlevelButtonCkick?.Invoke());

            _levelUIFeature.LevelCompleteView.UIButton.onClick.AddListener(LoadNextLevel);

            _levelUIFeature.LevelFailedView.UIButton.onClick.AddListener(LoadNextLevel);

            _levelUIFeature.GamePlayView.UIButton.onClick.AddListener(LoadNextLevel);
            _levelUIFeature.ButtonNext.onClick.AddListener(LoadNextLevel);

            _levelFeature.OnLevelLoaded += (levelRefs) => ToggleLevelCompleteButtons(true);
        }

        private void ToggleLevelCompleteButtons(bool enabled)
        {
            _levelUIFeature.GamePlayScreenView.ButtonRestart.interactable = enabled;
            _levelUIFeature.GamePlayScreenView.ButtonRestart.interactable = enabled;
            _levelUIFeature.LevelCompleteView.UIButton.interactable = enabled;
            _levelUIFeature.LevelFailedView.UIButton.interactable = enabled;
            _levelUIFeature.GamePlayView.UIButton.interactable = enabled;
            _levelUIFeature.ButtonNext.interactable = enabled;
        }

        private void SwitchLoadingScreenColors()
        {
            var loadingScreen = _levelUIFeature.LoadingScreenView;

            if (loadingScreen.IsFirstLoad)
            {
                loadingScreen.IsFirstLoad = false;

                loadingScreen.MainBGImage.color = loadingScreen.DefaultColor;
                //loadingScreen.Logo.SetActive(false);

                loadingScreen.ProgreessBarBGImage.color = loadingScreen.ProgressBarBGColor;
                loadingScreen.ProgressBarFillImage.color = loadingScreen.ProgressBarFillColor;
            }
        }

        private void LoadNextLevel()
        {
            ToggleLevelCompleteButtons(false);
            _levelUIFeature.OnClickNextLevel?.Invoke();
        }

        private void UpdateStartTimer(string text)
        {
            _levelUIFeature.GamePlayView.TextField.text = text;

            _levelUIFeature.GamePlayView.TextField.DOFontSize(300f, 0.9f)
                                        .OnComplete(() => _levelUIFeature.GamePlayView.TextField.fontSize = 0);
        }

        private void TryShowCompleteView(int id)
        {
            float time = 0;
            DOTween.To(() => time, x => time = x, _levelUIFeature.DelayToShowCompleteView, _levelUIFeature.DelayToShowCompleteView)
                    .onComplete += () =>
                    {
                        _levelUIFeature.LevelCompleteView.TextField.text = "Level " + id;
                        _levelUIFeature.LevelCompleteView.UIView.Show();

                        if (_levelUIFeature.LevelCompleteViewRefs && _levelUIFeature.LevelCompleteViewRefs.ConfettiParticles != null)
                        {
                            foreach (var effect in _levelUIFeature.LevelCompleteViewRefs.ConfettiParticles)
                            {
                                effect.gameObject.SetActive(true);
                            }
                        }

                        _vibroFeature.OnVibrateLevelComplete?.Invoke();
                    };
        }


        private void HideCompleteView()
        {
            _levelUIFeature.LevelCompleteView.UIView.Hide();
        }

        private void ShowFailedView(int number)
        {
            _levelUIFeature.LevelFailedView.TextField.text = "Level " + number;
            _levelUIFeature.LevelFailedView.UIView.Show();
            _vibroFeature.OnVibrateLevelFailed?.Invoke();

            // TODO: включить кнопку Skip Level
            /*
            if (_levelUIFeature.SkipLevelButton != null)
            {
                ShowSkipButton();
            }
            */
        }

        private void HideFailedView()
        {
            _levelUIFeature.LevelFailedView.UIView.Hide();
        }

        private void ShowLoadingScreen()
        {
            _levelUIFeature.LoadingScreenView.ProgressBarFillImage.fillAmount = 0;
            _levelUIFeature.LoadingScreenView.UIView.Show();
        }

        private void ShowLoadingScreenWithoutProgressBar()
        {
            _levelUIFeature.LoadingScreenView.UIView.Show();
        }

        private void HideLoadingScreen()
        {
            _levelUIFeature.LoadingScreenView.UIView.Hide();

            if (_levelUIFeature.LoadingScreenView.IsFirstLoad)
            {
                this.DelayedCall(0.5f, SwitchLoadingScreenColors);
            }
        }

        private void UpdateProgreesBar(float value)
        {
            _levelUIFeature.LoadingScreenView.ProgressBarFillImage.fillAmount = value;
        }

        private void ShowGamePlayView()
        {
            _levelUIFeature.GamePlayView.UIView.Show();
        }

        private void HideGamePlayView()
        {
            _levelUIFeature.GamePlayView.UIView.Hide();
        }

        private void ShowSkipButton()
        {
            _levelUIFeature.SkipLevelButton.gameObject.SetActive(true);

            if (_levelUIFeature.NoAdtext) _levelUIFeature.NoAdtext.gameObject.SetActive(false);
        }

        private void HidewSkipButton(bool state)
        {
            _levelUIFeature.SkipLevelCanvasGroup.alpha = state == true ? 1 : 0.25f;
            _levelUIFeature.SkipLevelButton.interactable = state;
        }
    }
}
