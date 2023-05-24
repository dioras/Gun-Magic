using System;
using UnityEngine;

namespace _GAME.Preload
{
    public class PreloadFeature : MonoBehaviour
    {
        public string GameSceneKey = "GameScene";
        public LevelUIView.LoadingScreenRefs LoadingScreen;

        public Action OnGameSceneLoaded;
    }
}
