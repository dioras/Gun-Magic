using _GAME.LevelUIView;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace _GAME.Level
{
    public class LevelSpawnLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private LevelUIFeature _levelUIFeature;
        private GameFeature _gameFeature;

        private int _currentLevelIndex = 0;
        private int _currentLoop = 0;

        private bool _complete = false;
        private bool _failed = false;

        private bool _levelLoadProcessing = false;

        private LevelData _levelData;

        private AsyncOperationHandle _handle;

        private WaitForSeconds _waiter;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _gameFeature = GameFeature.Instance;

            _waiter = new WaitForSeconds(0.5f);
        }

        private void OnEnable()
        {
            // _levelFeature.OnLoadNextLevel += () =>
            _levelUIFeature.OnClickNextLevel += () =>
            {
                if (_levelFeature.DebugMode)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    SpawnNewLevel(_currentLevelIndex);
                }
            };

            _levelFeature.OnLevelFailed += LevelFialed;
            _levelFeature.OnLevelComplete += LevelComplete;
            _levelFeature.OnLevelSkip += LevelSkip;
            _levelFeature.OnGetLevelKey += GetLevelKey;

            _levelUIFeature.OnRestartlevelButtonCkick += () => _levelUIFeature.OnClickNextLevel?.Invoke();
            _levelUIFeature.OnSkipLevelButtonClick += LevelSkip;
            _levelUIFeature.OnTapToStart += TapToStart;
            _levelFeature.OnSkipLevel += LevelSkip;

            _levelFeature.OnUpdateCurrentLevelIndex += (value) => _currentLevelIndex += value;

            _levelFeature.OnGetLevelData += () => { return _levelData; };

            _levelFeature.OnLoadData += (data) =>
            {
                _levelData = data;
                if (_levelData == null) _levelData = new LevelData();
                Init();
            };

            _levelFeature.OnSetlevelForDebug += SetLevel;
        }

        private void Init()
        {
            _currentLevelIndex = _levelData.CurrentLevelIndex;

            _currentLoop = _levelData.CurrentLevelLoop;

            _levelFeature.LevelLoadCount = _levelData.LevelLoadCount;

            SpawnNewLevel(_currentLevelIndex);
        }

        private void SpawnNewLevel(int index)
        {
            /*
            if (_levelFeature.NextPreloadLevel != null)
            {
                ClearLevel();

                _levelFeature.NextPreloadLevel.gameObject.SetActive(true);
                _levelFeature.Level = _levelFeature.NextPreloadLevel;

                _levelFeature.NextPreloadLevel = null;

                return;
            }
            */

            if (_levelFeature.DebugMode)
            {
                if (_levelFeature.Level) _levelFeature.OnLevelLoaded?.Invoke(_levelFeature.Level);
                else Debug.LogError("Level is null, set Level reference or uncheck DebugMode checkbox");

                return;
            }

            if (!_levelLoadProcessing)
            {
                _levelLoadProcessing = true;

                _complete = false;
                _failed = false;

                ClearLevel();

                System.GC.Collect();

                _levelFeature.OnSpawnNextLevel?.Invoke();

                string levelkey = null;

                if (!_levelFeature.LevelCollection.IsTestMode)
                {
                    levelkey = GetLevelKey(index);
                }
                else /*if (_levelFeature.LevelCollection.TestLevelKey != null)*/
                {
#if UNITY_EDITOR 
                    levelkey = _levelFeature.LevelCollection.TestLevelKey;
#elif !UNITY_EDITOR
                    levelkey = GetLevelKey(index);
#endif
                }

                if (_levelFeature.GenerateLevelFromJSON)
                    GenerateLevelFromJSON(levelkey);
                else
                    StartCoroutine(LoadLevelFromPrefab(levelkey));
            }
        }

        private async void GenerateLevelFromJSON(string levelkey)
        {
            _levelUIFeature.OnUpdateProgressBar?.Invoke(Random.Range(0.45f, 0.89f)); // 0.5f - 50 percents;

            var task = _levelFeature.OnGenerateLevelFromJSON.Invoke(levelkey);

            await task;

            CompleteLoading(_levelFeature.LevelGameObject);
        }


        private IEnumerator LoadLevelFromPrefab(string key)
        {
            if (_handle.IsValid())
            {
                Addressables.Release(_handle);
            }

            _handle = Addressables.InstantiateAsync(key);

            while (!_handle.IsDone)
            {
                _levelUIFeature.OnUpdateProgressBar?.Invoke(_handle.PercentComplete);

                yield return _handle;
            }

            if (_handle.Status == AsyncOperationStatus.Succeeded)
            {
                CompleteLoading((GameObject)_handle.Result);
            }
        }

        private string GetLevelKey(int index)
        {
            _levelFeature.LevelLoopCount = Mathf.CeilToInt(index / _levelFeature.LevelCollection.Levels.Length);

            if (_currentLoop != _levelFeature.LevelLoopCount)
            {
                _currentLoop = _levelFeature.LevelLoopCount;
                index += _levelFeature.CountLevelSkipAfterFirstLoop;
                _currentLevelIndex = index;

                _levelData.CurrentLevelLoop = _currentLoop;
                _levelData.CurrentLevelIndex = _currentLevelIndex;
                _levelFeature.OnSaveLevelData?.Invoke();
            }

            return _levelFeature.LevelCollection.Levels[index % _levelFeature.LevelCollection.Levels.Length];
        }

        private void CompleteLoading(GameObject levelGo)
        {
            Addressables.InstantiateAsync(_levelFeature.LevelCollection.LevelBaseKey).Completed += (handle) =>
            {
                _levelFeature.LevelBase = handle.Result.GetComponent<LevelBaseRefs>();

                levelGo.transform.SetParent(_levelFeature.LevelBase.transform);

                var level = levelGo.GetComponent<LevelRefs>();

                _levelUIFeature.OnUpdateProgressBar?.Invoke(1.0f); // 1.0f - 100 percents;

                if (level)
                {
                    StartCoroutine(UpdateLevel(level));
                }
                else
                {
                    Debug.LogError("Level dosn't have component LevelRefs");
                }
            };
        }

        private IEnumerator UpdateLevel(LevelRefs level)
        {
            yield return _waiter;

            _levelFeature.Level = level;
            _levelFeature.CurrentLevel = GetLevelNumber();
            _levelFeature.CurrentRealLevelNumber = GetRealLevelNumber();

            _levelUIFeature.TapToStartView.TextField.text = $"LEVEL {_levelFeature.CurrentLevel}";

            _levelFeature.LevelLoadCount++;

            _levelData.LevelLoadCount = _levelFeature.LevelLoadCount;

            _levelFeature.OnLevelLoaded?.Invoke(level);

            _levelUIFeature.OnLevelLoaded?.Invoke();

            _levelLoadProcessing = false;

            _levelFeature.LevelTime = 0;
            _levelFeature.PlatformTime = 0;

            if (_levelFeature.StartInBattleMode) //TODO: костыль для тестирования оружия. Отрефакторить
            {
                var _weaponHubFeature = GameFeature.WeaponHubFeature;
                var weapon = _weaponHubFeature.Settings.AvailableWeapons[_levelFeature.TestWeaponIndex];
                _weaponHubFeature.CraftedWeapon = Instantiate(weapon, _weaponHubFeature.Refs.WeaponHolder.transform);
                _weaponHubFeature.CraftedWeapon.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                _weaponHubFeature.Refs.TableTransform.gameObject.SetActive(false);

                _gameFeature.OnTransitionToLevel?.Invoke();
            }
        }

        private void TapToStart()
        {
            StartCoroutine("TimeTick");
        }

        private IEnumerator TimeTick()
        {
            while (true)
            {
                yield return _waiter;
                _levelFeature.LevelTime += 0.5f;
                _levelFeature.PlatformTime += 0.5f;
            }
        }

        private void ClearLevel()
        {
            if (_levelFeature.Level != null)
            {
                var go = _levelFeature.LevelBase.gameObject;
                go.SetActive(false);

                var wronglevel = FindObjectOfType<LevelRefs>();

                if (wronglevel && wronglevel != _levelFeature.Level)
                {
                    Addressables.ReleaseInstance(wronglevel.transform.parent.gameObject);
                }

                _levelFeature.Level = null;

                this.DelayedCall(5, () => Addressables.ReleaseInstance(go), false);
            }
        }

        private void ClearLevel(LevelRefs level)
        {
            Addressables.Release(level.gameObject);

            _levelFeature.Level = null;
        }

        private void LevelFialed()
        {
            if (!_complete)
            {
                _failed = true;

                StopCoroutine("TimeTick");

                if (_levelFeature.OnGetLevelProgress != null)
                {
                    _levelFeature.LevelProgress = _levelFeature.OnGetLevelProgress.Invoke();
                }

                _levelFeature.OnLevelEnd?.Invoke(false);

                _levelUIFeature.OnFailedLevel?.Invoke(_currentLevelIndex + 1); // _currentLevelIndex - is index of level. Number of level = index + 1;
            }
        }

        private void LevelComplete()
        {
            if (!_failed)
            {
                _complete = true;

                StopCoroutine("TimeTick");

                _levelFeature.LevelProgress = 100;

                _levelFeature.OnLevelEnd?.Invoke(true);

                _levelUIFeature.OnCompleteLevel?.Invoke(_currentLevelIndex + 1); // _currentLevelIndex - is index of level. Number of level = index + 1;

                _currentLevelIndex++;

                _levelFeature.OnPreloadNextLevel?.Invoke(_currentLevelIndex);

                _levelData.CurrentLevelIndex = _currentLevelIndex;
                _levelFeature.OnSaveLevelData?.Invoke();
            }
        }

        private void LevelSkip()
        {
            _complete = false;
            _failed = false;

            StopCoroutine("TimeTick");

            if (_levelFeature.OnGetLevelProgress != null)
            {
                _levelFeature.LevelProgress = _levelFeature.OnGetLevelProgress.Invoke();
            }

            _currentLevelIndex++;

            _levelData.CurrentLevelIndex = _currentLevelIndex;
            _levelFeature.OnSaveLevelData?.Invoke();

            _levelUIFeature.OnClickNextLevel?.Invoke();
        }

        private int GetLevelNumber()
        {
            if (_currentLoop > 0) return _currentLevelIndex + 1 - _levelFeature.CountLevelSkipAfterFirstLoop;
            return _currentLevelIndex + 1;
        }

        private int GetRealLevelNumber()
        {
            return _currentLevelIndex % _levelFeature.LevelCollection.Levels.Length + 1;
        }

        private void SetLevel(int levelNumber)
        {
            if (levelNumber < 1) levelNumber = 1;
            _levelData.CurrentLevelIndex = levelNumber - 1;
        }
    }
}