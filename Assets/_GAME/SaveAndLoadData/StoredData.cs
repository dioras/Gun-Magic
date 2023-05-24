using System;

namespace _GAME.SaveAndLoadData
{
    [Serializable]
    public class StoredData
    {
        public float dataVersion;
                
        //Audio data
        public bool SoundsOn = true;
        public bool MusicOn = true;
        public float trackVolumeInPercent;
        public float soundsVolumeInPercent;

        //Vibro data
        public bool VibrateOn = true;

        //Wallet data
        public int CoinsCount = 0;

        //Level data
        public int CurrentLevelIndex = 0;
        public int CurrentLevelLoop = 0;
        public int LevelLoadCount = 0;
        public Level.LevelData LevelData;

        //Shop data
        public ShopData[] ShopData;

        public RateUs.RateUsData RateUsData;
    }
}