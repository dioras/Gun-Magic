using UnityEngine;

namespace _GAME.Shop
{
    public class UpdatePreviewLogic : MonoBehaviour
    {
        private ShopFeature _shopFeature;

        private void Awake()
        {
            _shopFeature = GameFeature.ShopFeature;
        }

        private void OnEnable()
        {
            _shopFeature.OnShopElementButtonClick += UpdatePreview;
        }

        private void UpdatePreview(int index)
        {
            var shopInfo = _shopFeature.Shops[_shopFeature.CurrentOpenShopIndex];
            var prod = shopInfo.Shop.Products[index];

            _shopFeature.OnUpdateShopPreview?.Invoke(prod.Entity);
        }
    }
}
