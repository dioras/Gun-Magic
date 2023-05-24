using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _GAME.Level
{
    public class PreloadNextLevelLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private AsyncOperationHandle<GameObject> _asyncOperationHandle;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnPreloadNextLevel += PreloadLevel;
        }

        private async void PreloadLevel(int levelIndex)
        {
            var levelKey = _levelFeature.OnGetLevelKey(levelIndex);

            if (_asyncOperationHandle.IsValid()) Addressables.Release(_asyncOperationHandle);

            _asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(levelKey);
            _asyncOperationHandle.Completed += Hide;

            await _asyncOperationHandle.Task;

            if (_asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Level loaded");

                GameObject gameObjectPrefab = _asyncOperationHandle.Result;
                Instantiate(gameObjectPrefab);
                Addressables.Release(_asyncOperationHandle);
            }
        }

        private void Hide(AsyncOperationHandle<GameObject> obj)
        {
            var levevGo = obj.Result;
            levevGo.SetActive(false);

            var level = levevGo.GetComponent<LevelRefs>();

            if (level)
                _levelFeature.NextPreloadLevel = level;

            //Addressables.Release(_asyncOperationHandle);
        }
    }
}