using System.Collections.Generic;
using UnityEngine;

namespace _GAME
{
    [CreateAssetMenu(fileName = "PlatrformData", menuName = "GAME settings/PlatformData")]
    public class PlatformData : ScriptableObject
    {
        public List<PrefabData> PlatformPrefabDatas = new List<PrefabData>();
        
        public List<PrefabData> PlatformBuildingPrefabDatas = new List<PrefabData>();

        public List<PrefabData> PlatformPropPrefabDatas = new List<PrefabData>();

        public List<PrefabData> EnemyPrefabDatas = new List<PrefabData>();

        public List<PrefabData> EnemyAlternativePrefabDatas = new List<PrefabData>();

        public Vector3[] EnemyLocalPositions;

        public float PosX = 1.7f;

//#if UNITY_EDITOR
//        [Button("Collect Datas")]
//        public void Collect()
//        {
//            PlatformBuildingPrefabDatas.Clear();
//            PlatformPropPrefabDatas.Clear();
//            EnemyPrefabDatas.Clear();

//            foreach (var prefab in PlatformBuildingPrefabs)
//            {
//                PlatformBuildingPrefabDatas.Add(new PrefabData
//                {
//                    Prefab = prefab,
//                    LocalPosition = Vector3.zero
//                }) ;
//            }

//            foreach (var prefab in PlatformPropPrefabs)
//            {
//                PlatformPropPrefabDatas.Add(new PrefabData
//                {
//                    Prefab = prefab,
//                    LocalPosition = Vector3.zero
//                });
//            }

//            foreach (var prefab in EnemiesPrefabs)
//            {
//                EnemyPrefabDatas.Add(new PrefabData
//                {
//                    Prefab = prefab,
//                    LocalPosition = Vector3.zero
//                });
//            }
//        }
//#endif
    }

    [System.Serializable]
    public class PrefabData
    {
        public string VisibleName = "";
        public GameObject Prefab;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public bool SetSelected = false;
    }
}
