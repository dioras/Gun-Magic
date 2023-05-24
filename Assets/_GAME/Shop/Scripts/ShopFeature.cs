using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace _GAME.Shop
{
    public class ShopFeature : MonoBehaviour
    {
        [Header("UI Views")]
        public UIView[] ViewsForClose;
        public UIView ShopView;

        [Header("Any Buttons")]
        public ShopEnterButtonRefs[] ButtonsOpenShop;
        public Button ButtonCloseShop;
        public Button ButtonRandomItemOpen;
        public Button ButtonBuyRandomItem;

        public ButtonTabViewRefs[] TabButtons;

        [Header("TextBoxes")]
        public TMPro.TMP_Text PriceTextBox;
        public TMPro.TMP_Text ShopNameTextBox;

        [Header("Shop category panels")]
        public ShopCategoryViewRefs[] ShopButtonsPanels;

        [Header("Shops")]
        public ShopInfo[] Shops;

        [Header("Prefabs")]
        public ShopElementViewRefs IconPrefab;

        [Header("Shop Settings")]
        public float TimeToAnimateRandom = 0.05f;
        public float TimeToAnimateLastRandom = 0.5f;
        public int AnimateCircleCount = 3;

        [Header("Scene references")]
        public GameObject Preview;
        public ShopPreviewRefs[] ShopPreviews;

        [Header("Runtime References")]
        public List<ShopElementViewRefs> Icons = new List<ShopElementViewRefs>();

        public ShopElementViewRefs ActiveShopElementView;
        public ShopCategoryViewRefs ActiveShopPanel;

        public int CurrentOpenShopIndex;

        public Action OnRundomByAdsButtonClick;
        public Action OnUpdateOpenShopButtons;
        public Action<ShopInfo> OnUpdateBuyButtonInteractableState;
        public Action<bool> OnUpdateRandomOpenByAdsButtonInteractableState;
        public Action OnButtonBuyClick;
        public Action<int> OnShopElementButtonClick;
        public Action<int> OnOpenShop;
        public Action OnActivateRandomShopItem;
        public Action<ShopData[]> OnLoadShopData;
        public Func<ShopData[]> OnGetShopData;
        public Func<int> OnGetMoneyFromWallet;
        /// <summary>
        /// Be called before OnBuyFromWallet action, before withdraw money from the wallet 
        /// </summary>
        public Action OnStartBuy;
        public Action<int> OnBuyFromWallet;
        public Action OnBuyRewarded;
        public Action OnSave;
        public Action OnDataLoaded;

        public Action<int> OnSkinSelected;
        public Action OnPlayOpenSkinSound;

        public Action<AssetReference> OnUpdateShopPreview;
    }

    [System.Serializable]
    public class ShopInfo
    {
        public ShopElements Shop;
        public bool IsFirstBuy = true;
        public int LastActiveShopElementIndex = 0;
        public int LastTabIndex = 0;
        public int CurrentPrice;
    }
}
