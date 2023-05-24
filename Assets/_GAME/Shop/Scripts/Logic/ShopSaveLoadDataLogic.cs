using System.Linq;
using UnityEngine;

namespace _GAME.Shop
{
    public class ShopSaveLoadDataLogic : MonoBehaviour
    {
        private ShopFeature _shopFeature;

        private void Awake()
        {
            _shopFeature = GameFeature.ShopFeature;
        }

        private void OnEnable()
        {
            _shopFeature.OnLoadShopData += LoadData;
            _shopFeature.OnGetShopData += SaveData;
        }

        private ShopData[] SaveData()
        {
            ShopData[] shopDataArray = new ShopData[_shopFeature.Shops.Length];

            int index = 0;

            for (int i = 0; i < _shopFeature.Shops.Length; i++)
            {
                var shopInfo = _shopFeature.Shops[i];

                var shopData = new ShopData
                {
                    ShopId = shopInfo.Shop.Id,
                    LastActiveShopElementIndex = shopInfo.LastActiveShopElementIndex,
                    CurrentPrice = shopInfo.CurrentPrice,
                    IsFirstBuy = shopInfo.IsFirstBuy,
                    LastTabOpen = shopInfo.LastTabIndex
                };

                shopData.ProductDatas = new ProductData[shopInfo.Shop.Products.Length];

                for (int j = 0; j < shopInfo.Shop.Products.Length; j++)
                {
                    var prodactData = new ProductData
                    {
                        Locked = shopInfo.Shop.Products[j].Locked,
                        ShopItemName = shopInfo.Shop.Products[j].Name
                    };

                    shopData.ProductDatas[j] = prodactData;
                }

                shopDataArray[i] = shopData;
                index++;
            }

            return shopDataArray;
        }

        private void LoadData(ShopData[] shopData)
        {
            if (shopData == null || shopData.Length == 0) return;

            for (int i = 0; i < _shopFeature.Shops.Length; i++)
            {
                var shopInfo = _shopFeature.Shops[i];

                var shopSavedData = shopData.FirstOrDefault(s => s.ShopId == shopInfo.Shop.Id);

                if (shopSavedData == null) break;

                foreach (var product in shopInfo.Shop.Products)
                {
                    var productData = shopSavedData.ProductDatas.FirstOrDefault(s => s.ShopItemName == product.Name);

                    if (productData != null) product.Locked = productData.Locked;
                }

                _shopFeature.Shops[i].LastActiveShopElementIndex = shopSavedData.LastActiveShopElementIndex;
                _shopFeature.Shops[i].CurrentPrice = shopSavedData.CurrentPrice;
                _shopFeature.Shops[i].IsFirstBuy = shopSavedData.IsFirstBuy;
                _shopFeature.Shops[i].LastTabIndex = shopSavedData.LastTabOpen;
            }

            _shopFeature.OnDataLoaded?.Invoke();
        }
    }
}
