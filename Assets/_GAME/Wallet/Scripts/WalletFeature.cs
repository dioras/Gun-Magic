using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Wallet
{
    public class WalletFeature : MonoBehaviour
    {
        public System.Action OnDataLoaded;
        public System.Action OnSaveData;
        public System.Action<int> OnBuy;
        public System.Action<int> OnAddCoins;
        public System.Action OnGetRewardedCoins;
        public System.Action<bool> OnUpdateRewardedMoneyButtonState;
        public System.Action OnPlaySound;

        public ShowCoinsRefs CoinsAddedView;
        public ShowCoinsRefs CoinsInWalletView;

        public Button ButtonGetRewadedMoney;
        public GetRewardedMoneyButtonRefs RewardedMoneyButtonRefs;

        public CoinsForAnimationRefs CoinsForAnimation;

        public int CurrentReward = 50;
        public int CoinsInWallet;
        public int RewardedMultiply = 3;

        public float DelayToStartAddCoins = 1.2f;
        public float VolumeCoins = 0.3f;

        public WalletTimes WalletTimes;
    }

    [System.Serializable]
    public class WalletTimes
    {
        public float DelayToStartAccrualCoins = 2.35f;
        public float DelayToStartAccrualCoinsAfterReward = 2.35f;
        public float DelayToMoveCoinsInWallet = 1.2f;
        public float DurationAccrualCoins = 0.5f;
    }
}
