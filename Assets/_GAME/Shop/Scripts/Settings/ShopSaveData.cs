namespace _GAME
{
    [System.Serializable]
    public class ShopData
    {
        public int ShopId;

        public ProductData[] ProductDatas;

        public int LastActiveShopElementIndex;

        public int LastTabOpen;

        public int CurrentPrice;

        public bool IsFirstBuy;
    }

    [System.Serializable]
    public class ProductData
    {
        public string ShopItemName;

        public bool Locked;
    }
}
