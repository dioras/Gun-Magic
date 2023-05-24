using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _GAME.Shop
{
    public class UpdateShopViewIconsLogic : MonoBehaviour
    {
        private ShopFeature _shopFeature;

        private WaitForSeconds _waiter;

        private void Awake()
        {
            _shopFeature = GameFeature.ShopFeature;
        }

        private void OnEnable()
        {
            _shopFeature.OnOpenShop += (i) => StartCoroutine(UpdateIcons(i));
            _shopFeature.OnShopElementButtonClick += (i) => ActivateButton(i, false);
            _shopFeature.OnActivateRandomShopItem += ActivateRandomButton;

            _shopFeature.ButtonRandomItemOpen.onClick.AddListener(() => _shopFeature.OnRundomByAdsButtonClick?.Invoke());

            _shopFeature.OnUpdateRandomOpenByAdsButtonInteractableState += UpdateRandomOpenButtonState;

            SubscribeTabButtons();

            _waiter = new WaitForSeconds(_shopFeature.TimeToAnimateRandom);
        }

        private void ActivateRandomButton()
        {
            _shopFeature.ButtonCloseShop.interactable = false;
            _shopFeature.ButtonBuyRandomItem.interactable = false;
            //_shopFeature.ButtonRandomItemOpen.interactable = false;

            _shopFeature.OnUpdateRandomOpenByAdsButtonInteractableState?.Invoke(false);

            foreach (var tabView in _shopFeature.TabButtons)
            {
                tabView.TabButton.interactable = false;
            } 

            List<int> indexes = new List<int>();

            int count = _shopFeature.ActiveShopPanel.Icons.Count;

            for (int i = 0; i < count; i++)
            {
                if (_shopFeature.ActiveShopPanel.Icons[i].Locked)
                {
                    indexes.Add(i);
                }
            }

            if (indexes.Count > 0) StartCoroutine(Animate(indexes));
        }

        private IEnumerator Animate(List<int> indexes)
        {
            int randomIndex = indexes[Random.Range(0, indexes.Count)];
            int cycles = _shopFeature.AnimateCircleCount;

            for (int counter = 0; counter < cycles; counter++)
            {
                foreach (int index in indexes)
                {
                    ActivateButton(index, true);

                    if (counter == (cycles - 1) && randomIndex == index)
                    {
                        UnLockItem(index);
                        yield break;
                    }

                    if (counter == (cycles - 1))
                    {
                        yield return new WaitForSeconds(_shopFeature.TimeToAnimateLastRandom);
                    }
                    else
                    {
                        yield return _waiter;
                    }
                }
            }
        }

        private void UnLockItem(int index)
        {
            _shopFeature.OnPlayOpenSkinSound.Invoke();

            var iconView = _shopFeature.ActiveShopPanel.Icons[index];

            iconView.Locked = false;

            _shopFeature.OnUpdateBuyButtonInteractableState.Invoke(_shopFeature.Shops[_shopFeature.CurrentOpenShopIndex]);

            iconView.LockImage.enabled = iconView.Locked;
            iconView.Button.interactable = !iconView.Locked;

            iconView.Icon.enabled = !iconView.Locked;

            var shopInfo = _shopFeature.Shops[_shopFeature.CurrentOpenShopIndex];

            shopInfo.Shop.Products[iconView.Index].Locked = false;
            shopInfo.LastActiveShopElementIndex = iconView.Index;

            _shopFeature.OnShopElementButtonClick?.Invoke(iconView.Index);
            _shopFeature.OnSkinSelected?.Invoke(iconView.Index);

            _shopFeature.ButtonCloseShop.interactable = true;

            //_shopFeature.OnBuyFromWallet?.Invoke(shopInfo.CurrentPrice);  // send action to wallet to withdraw money 

            foreach (var tabView in _shopFeature.TabButtons)
            {
                tabView.TabButton.interactable = true;
            }
        }

        private void ActivateButton(int index, bool inner)
        {
            ShopElementViewRefs iconView = null;
            
            if (inner)
            {
                iconView = _shopFeature.ActiveShopPanel.Icons[index];
            }
            else
            {
                iconView = _shopFeature.Icons[index];
            }
            
            _shopFeature.Shops[_shopFeature.CurrentOpenShopIndex].LastActiveShopElementIndex = iconView.Index;

            DeactivateButton(_shopFeature.ActiveShopElementView);
            ActivateButton(iconView);
        }

        private void ActivateButton(ShopElementViewRefs iconView)
        {
            iconView.BG.color = iconView.ActiveColor;
            _shopFeature.ActiveShopElementView = iconView;
        }

        private void DeactivateButton(ShopElementViewRefs iconView)
        {
            iconView.BG.color = iconView.BaseColor;
            _shopFeature.ActiveShopElementView = null;
        }

        private void ClearCategoryIcons()
        {
            foreach (var icon in _shopFeature.Icons)
            {
                Destroy(icon.gameObject);
            }

            _shopFeature.Icons.Clear();

            foreach (var panelView in _shopFeature.ShopButtonsPanels)
            {
                foreach (var icon in panelView.Icons)
                {
                    Destroy(icon.gameObject);
                }

                panelView.Icons.Clear();
            }
        }

        private IEnumerator UpdateIcons(int shopIndex)
        {
            var shopInfo = _shopFeature.Shops[shopIndex];

            int panelIndex = 0;

            var activePanel = _shopFeature.ShopButtonsPanels[panelIndex];

            _shopFeature.ActiveShopPanel = _shopFeature.ShopButtonsPanels[panelIndex];

            ClearCategoryIcons();
            CloseTabs();
            ShowTabs(shopIndex, _shopFeature.Shops[shopIndex].LastTabIndex);
            ShowActivePanel(shopIndex);

            _shopFeature.ButtonBuyRandomItem.gameObject.SetActive(false);
            _shopFeature.ButtonRandomItemOpen.gameObject.SetActive(false);

            for (int i = 0; i < shopInfo.Shop.Products.Length; i++)
            {
                //TODO сделать каждые 9, а не просто 9
                if (i == shopInfo.Shop.ItemCountInCategory /*== 0*/)
                {
                    panelIndex++;
                    activePanel = _shopFeature.ShopButtonsPanels[panelIndex];
                }

                var prod = shopInfo.Shop.Products[i];
                var iconView = Instantiate(_shopFeature.IconPrefab, activePanel.transform);
                iconView.transform.localScale = Vector3.one;

                iconView.LockImage.enabled = prod.Locked;
                iconView.Index = i;
                iconView.Locked = prod.Locked;
                iconView.Button.interactable = !prod.Locked;
                iconView.Icon.enabled = !iconView.Locked;
                iconView.Button.onClick.AddListener(() => _shopFeature.OnShopElementButtonClick?.Invoke(iconView.Index));

                if (shopInfo.LastActiveShopElementIndex == iconView.Index)
                {
                    ActivateButton(iconView);
                }

                _shopFeature.Icons.Add(iconView);
                activePanel.Icons.Add(iconView);

                var op = Addressables.LoadAssetAsync<Sprite>(prod.Icon);
                op.Completed += handle => iconView.Icon.sprite = handle.Result;

                yield return op;
            }

            int countLocked = _shopFeature.ActiveShopPanel.Icons.Where(ic => ic.Locked == true).Count();

            _shopFeature.ButtonBuyRandomItem.gameObject.SetActive(countLocked != 0);
            _shopFeature.ButtonRandomItemOpen.gameObject.SetActive(countLocked != 0);
        }

        private void ShowActivePanel(int shopIndex)
        {
            for (int i = 0; i < _shopFeature.ShopButtonsPanels.Length; i++)
            {
                var index = _shopFeature.Shops[shopIndex].LastTabIndex;
                var tabPanel = _shopFeature.ShopButtonsPanels[i];

                if (i == index)
                {
                    tabPanel.gameObject.SetActive(true);
                    _shopFeature.ActiveShopPanel = tabPanel;
                }
                else
                {
                    tabPanel.gameObject.SetActive(false);
                }
            }
        }

        private void ShowTabs(int shopIndex, int activeTabIndex)
        {
            for (int i = 0; i < _shopFeature.Shops[shopIndex].Shop.CategoryNames.Length; i++)
            {
                _shopFeature.TabButtons[i].gameObject.SetActive(true);
                _shopFeature.TabButtons[i].BG.color = Color.white;
                _shopFeature.TabButtons[i].TabNameTextBox.text = _shopFeature.Shops[shopIndex].Shop.CategoryNames[i];

                if (i != activeTabIndex) 
                {
                    _shopFeature.TabButtons[i].BG.color = _shopFeature.TabButtons[i].NoActiveColor;
                }
            }
        }

        private void CloseTabs()
        {
            foreach (var tabButtonView in _shopFeature.TabButtons)
            {
                tabButtonView.gameObject.SetActive(false);
            }
        }

        private void SubscribeTabButtons()
        {
            for (int i = 0; i <  _shopFeature.TabButtons.Length; i++)
            {
                var tabButtonView = _shopFeature.TabButtons[i];
                var index = i;
                tabButtonView.TabButton.onClick.AddListener(() => OpenShopPanelAndClosePreviewPanel(tabButtonView, index));
            }
        }

        private void OpenShopPanelAndClosePreviewPanel(ButtonTabViewRefs tabButtonView, int tabIndex)
        {
            _shopFeature.ActiveShopPanel.gameObject.SetActive(false);
            tabButtonView.TargetShopPanel.gameObject.SetActive(true);
            tabButtonView.BG.color = Color.white;
            _shopFeature.ActiveShopPanel = tabButtonView.TargetShopPanel;
            _shopFeature.Shops[_shopFeature.CurrentOpenShopIndex].LastTabIndex = tabIndex;

            foreach (var tb in _shopFeature.TabButtons)
            {
                if (tb != tabButtonView) tb.BG.color = tb.NoActiveColor;
            }

            int countLocked = _shopFeature.ActiveShopPanel.Icons.Where(ic => ic.Locked == true).Count();

            _shopFeature.ButtonBuyRandomItem.gameObject.SetActive(countLocked != 0);
            _shopFeature.ButtonRandomItemOpen.gameObject.SetActive(countLocked != 0);
        }

        private void UpdateRandomOpenButtonState(bool state)
        {
            _shopFeature.ButtonRandomItemOpen.interactable = state;

            var canvasgroup = _shopFeature.ButtonRandomItemOpen.GetComponent<CanvasGroup>();

            if (canvasgroup)
            {
                canvasgroup.alpha = state == true ? 1 : 0.5f;
            }
        }

    }
}
