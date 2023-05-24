using System;
using System.Threading.Tasks;
using UnityEngine;

namespace _GAME.Level
{
    public class LevelFeature : MonoBehaviour
    {
        public LevelSettings Settings;
        public LevelCollection LevelCollection;

        public bool GenerateLevelFromJSON = false;

        public Func<string, Task> OnGenerateLevelFromJSON;

        public Action OnStartLevel;
        public Action OnLoadNextLevel;
        public Action OnSpawnNextLevel;
        public Action<LevelRefs> OnLevelLoaded;
        public Action OnLevelFailed;
        public Action OnLevelComplete;
        public Action OnLevelSkip;
        public Func<int> OnGetLevelProgress;
        public Action<int> OnUpdateCurrentLevelIndex;

        public Action<bool> OnLevelEnd;
        public Action OnSkipLevel;

        public Func<int> OnGetLevelIndex;
        public Func<int, string> OnGetLevelKey;

        public Action OnSaveLevelData;
        public Action<LevelData> OnLoadData;
        public Func<LevelData> OnGetLevelData;

        public Action<int> OnPreloadNextLevel;

        public Action<string> OnObjectUsed;
        public Action OnPlatformFinish;

        public event Action<int> OnSetlevelForDebug;

        public LevelRefs Level;
        public LevelBaseRefs LevelBase;
        public LevelRefs NextPreloadLevel;

        public GameObject LevelGameObject;

        public int CurrentLevel;

        public int CurrentRealLevelNumber;

        public int LevelLoadCount = 0;

        public int LevelLoopCount = 0;

        public int CountLevelSkipAfterFirstLoop;

        public int LevelProgress = 0;

        public float LevelTime;
        public float PlatformTime;

        public bool LevelFailed = false;

        public bool LevelComplete = false;

        public bool DebugMode = false;

        public bool StartInBattleMode = false;
        public int TestWeaponIndex;

        public bool WaitOtherTask = false;

        [Header("For testing")]
        public int LevelNumber;

        [ContextMenu("Set Level")]
        private void SetLevel()
        {
            OnSetlevelForDebug?.Invoke(LevelNumber);
        }
    }
}