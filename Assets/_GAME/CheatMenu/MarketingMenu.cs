using _GAME.Enemy;
using _GAME.Level;
using _GAME.LevelUIView;
using _GAME.Player;
using _GAME.Shop;
using _GAME.Wallet;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME
{
    public class MarketingMenu : MonoBehaviour
    {
        public GameObject[] ObjectToHide;

        public GameObject[] MarketingButtons;

        public bool ShowOnStart = true;
        public bool Visible = true;
        public bool AdVisible = true;
        public bool MenuVisible = true;

        public CanvasGroup CanvasGroup;

        public TMPro.TextMeshProUGUI HideUITextBox;
        public TMPro.TextMeshProUGUI NoAdsTextBox;
        public TMPro.TextMeshProUGUI LevelTime;

        public Button ButtonNextLevel;
        public Button ButtonPrevLevel;

        public Button HideMarketing;

        public Button ButtonShowMediationDebugger;

        private WalletFeature _walletFeature;
        private ShopFeature _shopFeature;
        private PlayerFeature _playerFeature;
        private LevelUIFeature _levelUIFeature;
        private LevelFeature _levelFeature;
        private EnemyFeature _enemyFeature;

        private void Awake()
        {
            _walletFeature = FindObjectOfType<WalletFeature>();
            _shopFeature = FindObjectOfType<ShopFeature>();
            _playerFeature = FindObjectOfType<PlayerFeature>();
            _levelUIFeature = GetComponent<LevelUIFeature>();
            _levelFeature = FindObjectOfType<LevelFeature>();
            _enemyFeature = FindObjectOfType<EnemyFeature>();

            ButtonShowMediationDebugger.interactable = false;
        }

        private void OnEnable()
        {
            ButtonNextLevel.onClick.AddListener(() => _levelFeature.OnSkipLevel?.Invoke());
            ButtonPrevLevel.onClick.AddListener(() =>
            {
                _levelFeature.OnUpdateCurrentLevelIndex?.Invoke(-2);
                _levelFeature.OnSkipLevel?.Invoke();
            });

            HideMarketing.onClick.AddListener(HideCheatMenu);

            /* TODO: включить Mediation Debugger Button
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // Show Mediation Debugger Button
                ButtonShowMediationDebugger.interactable = true;
                ButtonShowMediationDebugger.onClick.AddListener(() => MaxSdk.ShowMediationDebugger());
            };
            */
        }

        private void Start()
        {
            NoAdsTextBox.text = AdVisible ? "NO AD" : "Show AD";
            HideUITextBox.text = Visible ? "Hide UI" : "Show UI";
            CanvasGroup.alpha = MenuVisible ? .75f : 0f;

            if (!ShowOnStart)
                HideCheatMenu();
        }

        public void SwitchVisible()
        {
            Visible = !Visible;
            HideUITextBox.text = Visible ? "Hide UI" : "Show UI";

            foreach (var item in ObjectToHide)
            {
                item.SetActive(Visible);
            }
        }

        private void HideCheatMenu()
        {
            MenuVisible = !MenuVisible;

            CanvasGroup.alpha = MenuVisible ? .75f : 0f;

            foreach (var item in MarketingButtons)
            {
                item.SetActive(MenuVisible);
            }
        }

        public void ToogleAD()
        {
            AdVisible = !AdVisible;
            NoAdsTextBox.text = AdVisible ? "NO AD" : "Show AD";
        }

        public void AddMoney()
        {
            _walletFeature.OnAddCoins?.Invoke(1000);
            _shopFeature.OnUpdateOpenShopButtons?.Invoke();
            _shopFeature.OnUpdateBuyButtonInteractableState?.Invoke(_shopFeature.Shops[_shopFeature.CurrentOpenShopIndex]);
        }

        private void LateUpdate()
        {
            LevelTime.text = _levelFeature.LevelTime.ToString("F1");
        }
    }
}
