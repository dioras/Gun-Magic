using System.Linq;
using UnityEngine;

namespace _GAME.Shop
{
    public class OpenCloseShopLogic : MonoBehaviour
    {
        private ShopFeature _shopFeature;

        //private List<int> _buttonIds = new List<int>();

        //private Coroutine _openShopCoroutine;

        private void Awake()
        {
            _shopFeature = GameFeature.ShopFeature;
        }

        private void OnEnable()
        {
            for (int i = 0; i < _shopFeature.ButtonsOpenShop.Length; i++)
            {
                var but = _shopFeature.ButtonsOpenShop[i];

                but.ShopOpenButton.onClick.AddListener(() => ButtonOpenShopClicked(but.ShopIndex));
            }

            _shopFeature.ButtonCloseShop.onClick.AddListener(HideShop);
            _shopFeature.OnUpdateOpenShopButtons += UpdateOpenShopButtons;

            _shopFeature.OnOpenShop += UpdatePreview;
        }

        private void UpdatePreview(int shopIndex)
        {
            var lastIndex = _shopFeature.Shops[shopIndex].LastActiveShopElementIndex;
            var selectedProduct = _shopFeature.Shops[shopIndex].Shop.Products[lastIndex];

            _shopFeature.OnUpdateShopPreview?.Invoke(selectedProduct.Entity);
        }

        private void ButtonOpenShopClicked(int Id)
        {
            OpenShop(Id);
            //_buttonIds.Add(Id);

            //if (_openShopCoroutine != null) StopCoroutine(_openShopCoroutine);

            //_openShopCoroutine = this.DelayedCall(0.1f, () =>
            //{
            //    OpenShop(_buttonIds.Last());
            //    _buttonIds.Clear();
            //});
        }

        private void OpenShop(int shopIndex)
        {
            foreach (var view in _shopFeature.ViewsForClose)
            {
                view.Hide();
            }

            _shopFeature.ShopView.Show();
            _shopFeature.CurrentOpenShopIndex = shopIndex;
            _shopFeature.Preview.SetActive(true);
            _shopFeature.ShopPreviews[shopIndex].RootGameObject.SetActive(true);
            _shopFeature.OnOpenShop?.Invoke(shopIndex);

            _shopFeature.ShopNameTextBox.SetText(_shopFeature.Shops[shopIndex].Shop.Label);

            //int countLocked = _shopFeature.Shops[shopIndex].Shop.Products.Where(p => p.Locked == true).Count();

            //_shopFeature.ButtonBuyRandomItem.gameObject.SetActive(countLocked != 0);
            //_shopFeature.ButtonRandomItemOpen.gameObject.SetActive(countLocked != 0);
        }

        private void HideShop()
        {
            _shopFeature.Preview.SetActive(false);

            foreach (var prev in _shopFeature.ShopPreviews)
            {
                prev.RootGameObject.SetActive(false);
            }

            _shopFeature.ShopView.Hide();

            foreach (var view in _shopFeature.ViewsForClose)
            {
                view.Show();
            }

            _shopFeature.OnSave?.Invoke();

            UpdateOpenShopButtons();
        }

        private void UpdateOpenShopButtons()
        {
            foreach (var butRef in _shopFeature.ButtonsOpenShop)
            {
                int price = _shopFeature.Shops[butRef.ShopIndex].CurrentPrice;

                int countLocked = _shopFeature.Shops[butRef.ShopIndex].Shop.Products.Where(p => p.Locked == true).Count();

                butRef.ImageInfo.enabled = _shopFeature.OnGetMoneyFromWallet?.Invoke() >= price && countLocked > 0;
            }
        }
    }
}
