using System.Linq;
using UnityEngine;

namespace _GAME.Shop
{
    public class ShopBuyLogic : MonoBehaviour
    {
        private ShopFeature _shopFeature;

        private void Awake()
        {
            _shopFeature = GameFeature.ShopFeature;
        }

        private void OnEnable()
        {
            _shopFeature.ButtonBuyRandomItem.onClick.AddListener(UpdateCurrentPrice);
            _shopFeature.OnOpenShop += ShowCoinPrice;
            _shopFeature.OnUpdateBuyButtonInteractableState += UpdateBuyButtonState;
        }

        private void UpdateCurrentPrice()
        {
            var shopInfo = _shopFeature.Shops[_shopFeature.CurrentOpenShopIndex];

            _shopFeature.OnStartBuy.Invoke();
            _shopFeature.OnBuyFromWallet?.Invoke(shopInfo.CurrentPrice); //send an action to the wallet to withdraw money

            if (shopInfo.IsFirstBuy)
            {
                shopInfo.CurrentPrice = shopInfo.Shop.StartPrice;
                shopInfo.IsFirstBuy = false;
            }
            else
            {
                shopInfo.CurrentPrice += shopInfo.Shop.Increase;
            }

            UpdateBuyButtonState(shopInfo);

            _shopFeature.OnActivateRandomShopItem?.Invoke();
        }

        private void ShowCoinPrice(int shopIndex)
        {
            var shopInfo = _shopFeature.Shops[shopIndex];

            _shopFeature.PriceTextBox.SetText($"{shopInfo.CurrentPrice}");

            if (_shopFeature.OnGetMoneyFromWallet != null)
            {
                var money = _shopFeature.OnGetMoneyFromWallet.Invoke();

                _shopFeature.ButtonBuyRandomItem.interactable = money >= shopInfo.CurrentPrice;
            }

            //UpdateBuyButtonState(shopInfo);
        }

        private void UpdateBuyButtonState(ShopInfo shopInfo)
        {
            _shopFeature.PriceTextBox.SetText($"{shopInfo.CurrentPrice}");

            if (_shopFeature.OnGetMoneyFromWallet != null)
            {
                var money = _shopFeature.OnGetMoneyFromWallet.Invoke();

                _shopFeature.ButtonBuyRandomItem.interactable = money >= shopInfo.CurrentPrice;
            }

            /*Shops[_shopFeature.CurrentOpenShopIndex].Shop.Products*/
            int countLocked = _shopFeature.ActiveShopPanel.Icons.Where(ic => ic.Locked == true).Count();

            _shopFeature.ButtonBuyRandomItem.gameObject.SetActive(countLocked != 0);
            _shopFeature.ButtonRandomItemOpen.gameObject.SetActive(countLocked != 0);

            _shopFeature.ButtonCloseShop.interactable = true;
        }
    }
}
