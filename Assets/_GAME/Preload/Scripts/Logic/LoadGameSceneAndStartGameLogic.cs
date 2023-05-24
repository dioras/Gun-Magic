using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _GAME.Preload
{
    public class LoadGameSceneAndStartGameLogic : MonoBehaviour
    {
        private PreloadFeature _preloadFeature;

        private void Awake()
        {
            _preloadFeature = FindObjectOfType<PreloadFeature>();
        }
        private async void Start()
        {
            await LoadGameScene();
        }

        private async Task LoadGameScene()
        {
           var handle =  Addressables.LoadSceneAsync(_preloadFeature.GameSceneKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var scene = handle.Result;
                scene.ActivateAsync();
                UpdateLoadingScreen();
                _preloadFeature.OnGameSceneLoaded.Invoke();
                
                Addressables.Release(handle);
            }
        }

        private void UpdateLoadingScreen()
        {
            var levelUI = GameFeature.LevelUIFeature;
            levelUI.LoadingScreenView = _preloadFeature.LoadingScreen;
        }
    }
}
