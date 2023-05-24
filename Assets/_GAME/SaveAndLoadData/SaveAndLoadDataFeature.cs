using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _GAME.SaveAndLoadData
{
    public class SaveAndLoadDataFeature : MonoBehaviour
    {
        public Action OnPrepareDataForSaving;
        public Action OnLoadData;
        public Action OnSaveData;
        public Action OnDataLoaded;

        public float DataVersion;
        public float LastValidDataVersion;
        
        public bool LoadingEnabled;
        public bool SavingEnabled;

        [ReadOnly] public StoredData Data;
        [ReadOnly] public bool DataLoaded;

        public string PlayerPrefsKeySuffix = "Portals";

        //private readonly string playerPrefsKey = "StoredData";

        //public void TryLoadData()
        //{
        //    if (!LoadingEnabled)
        //        return;

        //    if (!PlayerPrefs.HasKey(playerPrefsKey))
        //    {
        //        Data = new StoredData();
        //        return;
        //    }

        //    var json = PlayerPrefs.GetString(playerPrefsKey);
        //    var storedData = JsonUtility.FromJson(json, typeof(StoredData)) as StoredData;
        //    if (storedData.dataVersion < LastValidDataVersion)
        //        return;

        //    Data = storedData;
        //    DataLoaded = true;
        //}

        //public void TrySaveData()
        //{
        //    if (!SavingEnabled)
        //        return;

        //    OnPrepareDataForSaving?.Invoke();

        //    Data.dataVersion = DataVersion;

        //    var json = JsonUtility.ToJson(Data);
        //    PlayerPrefs.SetString(playerPrefsKey, json);
        //}

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
                return;

            if (Application.platform != RuntimePlatform.IPhonePlayer &&
                Application.platform != RuntimePlatform.Android)
                return;

            OnSaveData?.Invoke();
        }

        private void OnApplicationQuit()
        {
            OnSaveData?.Invoke();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/DeleteAllSaveData")]
        private static void DeleteAllSaveData()
        {
            PlayerPrefs.DeleteAll();
        }
#endif
    }
}
