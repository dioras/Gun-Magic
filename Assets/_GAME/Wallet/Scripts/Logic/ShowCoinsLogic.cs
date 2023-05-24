using _GAME.Audio;
using _GAME.Level;
using DG.Tweening;
using UnityEngine;

namespace _GAME.Wallet
{
    public class ShowCoinsLogic : MonoBehaviour
    {
        private WalletFeature _walletFeature;
        private LevelFeature _levelFeature;
        private LevelUIView.LevelUIFeature _levelUIFeature;
        private SaveAndLoadData.SaveAndLoadDataFeature _saveAndLoadDataFeature;
        private AudioFeature _audioFeature;
        private WeaponHubFeature _weaponHubFeature;

        private void Awake()
        {
            _walletFeature = GameFeature.WalletFeature;
            _levelFeature = GameFeature.LevelFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _saveAndLoadDataFeature = GameFeature.SaveAndLoadDataFeature;
            _audioFeature = GameFeature.AudioFeature;
            _weaponHubFeature = GameFeature.WeaponHubFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelComplete += ShowAddedCoins;
            _levelFeature.OnLevelFailed += ShowAddedCoins;
            _levelUIFeature.OnClickNextLevel += Hide;

            _levelFeature.OnLevelLoaded += ShowWallet;
            _weaponHubFeature.OnMoveIntoPortal += Hide;

            _walletFeature.OnBuy += SpendCoins;
            _walletFeature.OnDataLoaded += LoadCoinsInWallet;
            _walletFeature.OnAddCoins += AddCoins;
            _walletFeature.OnGetRewardedCoins += SetRewardedMoney;
            _walletFeature.OnUpdateRewardedMoneyButtonState += SetButtonRewardedMoneyState;

            _walletFeature.ButtonGetRewadedMoney.onClick.AddListener(() =>SetButtonRewardedMoneyState(false));
        }

        private void LoadCoinsInWallet()
        {
            _walletFeature.CoinsInWallet = _saveAndLoadDataFeature.Data.CoinsCount;
        }

        private void ShowWallet(LevelRefs level)
        {
            ShowCoinsInWallet();
        }

        private void ShowAddedCoins()
        {
            ShowCoinsInWallet();
            _walletFeature.CoinsAddedView.CoinsTextBox.text = $"+{_walletFeature.CurrentReward}";
            _walletFeature.CoinsAddedView.UIView.Show();

            //TODO: включить награду за рекламу
            //_walletFeature.ButtonGetRewadedMoney.gameObject.SetActive(true);
            
            _levelUIFeature.ButtonNext.gameObject.SetActive(false);
            _levelUIFeature.LevelCompleteView.UIButton.gameObject.SetActive(true);

            _walletFeature.RewardedMoneyButtonRefs.TextBox.text = $"+{(_walletFeature.CurrentReward * _walletFeature.RewardedMultiply)}";

            UpdateCoinsWallet(_walletFeature.WalletTimes.DelayToMoveCoinsInWallet,
                              _walletFeature.WalletTimes.DelayToStartAccrualCoins);
        }

        private void SetButtonRewardedMoneyState(bool state)
        {
            var rb = _walletFeature.RewardedMoneyButtonRefs;

            if (!state)
            {
                rb.CanvasGroup.alpha = rb.Alpha;
                rb.Button.interactable = false;
            }
            else
            {
                rb.CanvasGroup.alpha = 1.0f;
                rb.Button.interactable = true;
            }
        }

        private void SetRewardedMoney()
        {
            _walletFeature.CurrentReward *= _walletFeature.RewardedMultiply;

            SetButtonRewardedMoneyState(false);
            
            _walletFeature.ButtonGetRewadedMoney.gameObject.SetActive(false);
            _levelUIFeature.ButtonNext.gameObject.SetActive(true);
            _levelUIFeature.LevelCompleteView.UIButton.gameObject.SetActive(false);

            _walletFeature.CoinsAddedView.CoinsTextBox.text = $"+{_walletFeature.CurrentReward}";

            UpdateCoinsWallet(0, _walletFeature.WalletTimes.DelayToStartAccrualCoinsAfterReward);
        }

        private void UpdateCoinsWallet(float delayToMoveCoinsInWallet, float delayCounter)
        {
            foreach (var img in _walletFeature.CoinsForAnimation.CoinImages)
            {
                img.gameObject.SetActive(true);
            }

            var delay = delayCounter;
            var duration = _walletFeature.WalletTimes.DurationAccrualCoins;

            var walletCache = _walletFeature.CoinsInWallet;
            _walletFeature.CoinsInWallet += _walletFeature.CurrentReward;
            _walletFeature.OnSaveData?.Invoke();

            _walletFeature.CoinsInWalletView.CoinsTextBox.DOCounter(walletCache, _walletFeature.CoinsInWallet, duration, false)
                                                         .SetDelay(delay);

            this.DelayedCall(delayToMoveCoinsInWallet, () =>
                                  {
                                      _walletFeature.CoinsInWalletView.Animator.SetTrigger("Animate");
                                      _walletFeature.OnPlaySound?.Invoke();
                                  });
        }

        private void ShowCoinsInWallet()
        {
            foreach (var img in _walletFeature.CoinsForAnimation.CoinImages)
            {
                img.gameObject.SetActive(false);
            }

            _walletFeature.CoinsInWalletView.CoinsTextBox.text = $"{_walletFeature.CoinsInWallet}";
            _walletFeature.CoinsInWalletView.UIView.Show();
        }

        private void Hide()
        {
            _walletFeature.CoinsAddedView.UIView.Hide();
            _walletFeature.CoinsInWalletView.UIView.Hide();
            _walletFeature.CurrentReward = 0;
        }

        private void SpendCoins(int count)
        {
            _walletFeature.CoinsInWallet -= count;
            _walletFeature.CoinsInWalletView.CoinsTextBox.text = $"{_walletFeature.CoinsInWallet}";
            _saveAndLoadDataFeature.Data.CoinsCount = _walletFeature.CoinsInWallet;
        }

        private void AddCoins(int count)
        {
            _walletFeature.CoinsInWallet += count;
            _walletFeature.CoinsInWalletView.CoinsTextBox.text = $"{_walletFeature.CoinsInWallet}";
            _saveAndLoadDataFeature.Data.CoinsCount = _walletFeature.CoinsInWallet;
        }
    }
}
