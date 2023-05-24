using UnityEngine;

namespace _GAME.SaveAndLoadData
{
    public class SaveLoadLogic : MonoBehaviour
    {
        private SaveAndLoadDataFeature _saveAndLoadDataFeature;

        private string playerPrefsKey = "StoredData_";

        private void Awake()
        {
            _saveAndLoadDataFeature = FindObjectOfType<SaveAndLoadDataFeature>();
            playerPrefsKey += _saveAndLoadDataFeature.PlayerPrefsKeySuffix; //.GetHashCode();
        }

        private void OnEnable()
        {
            _saveAndLoadDataFeature.OnLoadData += LoadData;
            _saveAndLoadDataFeature.OnSaveData += SaveData;
        }

        private void SaveData()
        {
            if (!_saveAndLoadDataFeature.SavingEnabled) return;

            _saveAndLoadDataFeature.OnPrepareDataForSaving?.Invoke();

            _saveAndLoadDataFeature.Data.dataVersion = _saveAndLoadDataFeature.DataVersion;

            var json = JsonUtility.ToJson(_saveAndLoadDataFeature.Data);
            PlayerPrefs.SetString(playerPrefsKey, json);
        }

        private void LoadData()
        {
            if (!_saveAndLoadDataFeature.LoadingEnabled)
                return;

            if (!PlayerPrefs.HasKey(playerPrefsKey))
            {
                _saveAndLoadDataFeature.Data = new StoredData();
                _saveAndLoadDataFeature.OnDataLoaded?.Invoke();
                return;
            }

            var json = PlayerPrefs.GetString(playerPrefsKey);
            var storedData = JsonUtility.FromJson(json, typeof(StoredData)) as StoredData;
            if (storedData.dataVersion < _saveAndLoadDataFeature.LastValidDataVersion)
                return;

            _saveAndLoadDataFeature.Data = storedData;
            _saveAndLoadDataFeature.DataLoaded = true;

            _saveAndLoadDataFeature.OnDataLoaded?.Invoke();
        }
    }
}
