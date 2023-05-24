using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _GAME.Shop
{
    [CreateAssetMenu(fileName = "ShopCollection", menuName = "GAME settings/ShopCollection")]
    public class ShopElements : ScriptableObject
    {
        public int Id;
        public string Label;

        public int StartPrice = 1000;
        public int Increase = 200;

        public int ItemCountInCategory = 9;

        public string[] CategoryNames;

        public ShopItem[] Products;
    }

    [System.Serializable]
    public class ShopItem
    {
        public string Name;

        public bool Locked = true;
        
        public AssetReference Icon;

        public AssetReference Entity;
    }
}
