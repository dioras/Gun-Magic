using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace _GAME.Level
{
    [CreateAssetMenu(fileName = "LevelCollection", menuName = "GAME settings/LevelCollection")]
    public class LevelCollection : ScriptableObject
    {
        public string LevelBaseKey;
        public string[] Levels;

        public bool IsTestMode = false;

        public string TestLevelKey;

#if UNITY_EDITOR

        //[TableList(ShowIndexLabels = false, ShowPaging = true, MaxScrollViewHeight = 650)]
        //public List<LevelPreset> LevelPresets = new List<LevelPreset>();

        [Button]
        public void CollectLevels()
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/_GAME/Level/Prefabs/Levels" });

            Levels = new string[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var name = Path.GetFileNameWithoutExtension(path);
                Levels[i] = name;
            }
        }
#endif
    }

    [System.Serializable]
    public class LevelPreset
    {
        [TableColumnWidth(20)]
        [LabelWidth(50)]
        [HorizontalGroup("LevelPresets")]
        public int Number;
        [TableColumnWidth(30)]
        [LabelWidth(80)]
        [HorizontalGroup("LevelPresets")] 
        public GameObject LevelPrefab;
    }
}
