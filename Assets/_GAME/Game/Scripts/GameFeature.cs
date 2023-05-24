using _GAME.Audio;
using _GAME.Enemy;
using _GAME.Level;
using _GAME.LevelUIView;
using _GAME.Player;
using _GAME.Preload;
using _GAME.RateUs;
using _GAME.SaveAndLoadData;
using _GAME.Shop;
using _GAME.Tutorial;
using _GAME.VibroService;
using _GAME.Wallet;
using System;
using UnityEngine;

namespace _GAME
{
    public class GameFeature : MonoBehaviour
    {
        public Action OnStartGame;
        public Action OnTransitionToLevel;

        public static SaveAndLoadDataFeature SaveAndLoadDataFeature;
        public static ShopFeature ShopFeature;
        public static WalletFeature WalletFeature;
        public static LevelFeature LevelFeature;
        public static VibroFeature VibroFeature;
        public static AudioFeature AudioFeature;
        public static PreloadFeature PreloadFeature;

        public static RateUsFeature RateUsFeature;
        public static PlayerFeature PlayerFeature;
        public static EnemyFeature EnemyFeature;
        public static WeaponHubFeature WeaponHubFeature;
        public static WeaponsFeature WeaponsFeature;

        public static LevelUIFeature LevelUIFeature;

        public static TutorialFeature TutorialFeature;

        public static GameFeature Instance;

        public bool IsTestMode = false;
    }
}
