using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _GAME
{
    public class PlatformRefs : MonoBehaviour
    {
#if UNITY_EDITOR
        public PlatformData PlatformData;
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlatformRefs))]
    public class PlatformRefsEditor : Editor
    {
        private PlatformRefs _platformRefs;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _platformRefs = (PlatformRefs)target;

            if (_platformRefs.PlatformData == null)
            {
                var guids = AssetDatabase.FindAssets("t:PlatformData", new[] { "Assets/_GAME/Level/Data" });
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _platformRefs.PlatformData = (PlatformData)AssetDatabase.LoadAssetAtPath(path, typeof(PlatformData));
            }

            if (GUILayout.Button("Clear Platform"))
            {
                ClearPlatform(_platformRefs);
            }

            if (_platformRefs.PlatformData != null)
            {
                EditorGUILayout.LabelField("Select platform");
                EditorGUILayout.BeginHorizontal();
                foreach (var data in _platformRefs.PlatformData.PlatformPrefabDatas)
                {
                    if (GUILayout.Button(Resources.Load<Texture>($"{data.Prefab.name}"), GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        var destroyList = new List<GameObject>();
                        
                        foreach (Transform child in _platformRefs.transform)
                        {
                            if (child.gameObject.name.Contains("Platf"))
                            {
                                destroyList.Add(child.gameObject);
                            }
                        }

                        DestroyGameobjects(destroyList);

                        var newPlatfom = (GameObject)PrefabUtility.InstantiatePrefab(data.Prefab, _platformRefs.transform);
                        newPlatfom.transform.localPosition = data.LocalPosition;
                    }
                }
                EditorGUILayout.EndHorizontal();

                DrawTableButtons(_platformRefs.PlatformData.PlatformBuildingPrefabDatas, "Buildings");
                DrawTableButtons(_platformRefs.PlatformData.PlatformPropPrefabDatas, "Props");
                DrawTableButtons(_platformRefs.PlatformData.EnemyAlternativePrefabDatas, "Alternative Enemies");
                DrawTableButtons(_platformRefs.PlatformData.EnemyPrefabDatas, "Enemies");

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Enemy packs");

                if (GUILayout.Button("Stand 2 Enemy"))
                {
                    StandEnemies(2);
                }

                if (GUILayout.Button("Stand 3 Enemy"))
                {
                    StandEnemies(3);
                }

                if (GUILayout.Button("Stand 4 Enemy"))
                {
                    StandEnemies(4);
                }

                if (GUILayout.Button("Stand 5 Enemy"))
                {
                    StandEnemies(5);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DestroyGameobjects(List<GameObject> destroyList)
        {
            while(destroyList.Count > 0)
            {
                for (int i = 0; i < destroyList.Count; i++)
                {
                    var go = destroyList[i];
                    DestroyImmediate(go);

                    if (go == null) destroyList.Remove(go);
                }
            }
        }

        private void StandEnemies(int count)
        {
            var enemieData = new List<PrefabData>();
            var positions = _platformRefs.PlatformData.EnemyLocalPositions.ToList();

            int length = _platformRefs.PlatformData.EnemyPrefabDatas.Count;

            for (int i = 0; i < count; i++)
            {
                enemieData.Add(_platformRefs.PlatformData.EnemyPrefabDatas[Random.Range(0, length)]);
            }

            var stage = _platformRefs.transform.GetComponentInChildren<Level.StageRefs>();
            stage.RemoveAllEnemies();
            var parent = stage.transform.GetChild(0);

            foreach (var data in enemieData)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(data.Prefab, parent);
                go.transform.localPosition = GetPosiiton(positions);
                var angles = go.transform.eulerAngles;
                angles.y = 180;
                go.transform.eulerAngles = angles;
                var enemy = go.GetComponent<Enemy.EnemyRefs>();

                if (enemy)
                {
                    enemy.transform.SetParent(parent);
                    stage.AddEnemyInCollection(enemy);
                }
            }
        }

        private Vector3 GetPosiiton(List<Vector3> positions)
        {
            Vector3 pos = positions[Random.Range(0, positions.Count)];

            //positions.Remove(pos);

            pos.x = Random.Range(-_platformRefs.PlatformData.PosX, _platformRefs.PlatformData.PosX);

            return pos;
        }

        private void DrawTableButtons(List<PrefabData> datas, string labelName)
        {
            int rowsCount = 5;
            int count = Mathf.CeilToInt((float)datas.Count() / rowsCount);
            int skipCount = 0;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(labelName);

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.BeginVertical();
                foreach (var data in datas.Skip(skipCount).Take(rowsCount))
                {
                    string name = data.VisibleName == "" ? CutString(data.Prefab.name) : data.VisibleName;

                    if (GUILayout.Button($"{name}"))
                    {
                        var parent = _platformRefs.transform;

                        var go = (GameObject)PrefabUtility.InstantiatePrefab(data.Prefab, parent);

                        if(data.SetSelected) Selection.activeGameObject = go;

                        go.transform.localPosition = data.LocalPosition;
                        go.transform.eulerAngles = data.LocalRotation;

                        var enemy = go.GetComponent<Enemy.EnemyRefs>();

                        if (enemy)
                        {
                            var stage = _platformRefs.transform.GetComponentInChildren<Level.StageRefs>();
                            parent = stage.transform.GetChild(0);
                            enemy.transform.SetParent(parent);

                            stage.AddEnemyInCollection(enemy);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                skipCount += rowsCount;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private string CutString(string inputString)
        {
            if (inputString.Contains("_"))
            {
                inputString = inputString.Replace('_', ' ');
            }

            return inputString;
        }

        private void ClearPlatform(PlatformRefs platform)
        {
            var listToDelete = new List<GameObject>();

            foreach (Transform child in platform.transform)
            {
                if (child.gameObject.name.Contains("Platf") ||
                    child.gameObject.name.Contains("Way Point")) continue;

                if (IsConnected(child.gameObject))
                {
                    listToDelete.Add(child.gameObject);
                }

            }

            var stageTr = platform.GetComponentInChildren<Level.StageRefs>().transform.GetChild(0);

            if (stageTr)
            {
                foreach (Transform child in stageTr)
                {
                    if (IsConnected(child.gameObject))
                    {
                        listToDelete.Add(child.gameObject);
                    }
                }
            }

            DeleteConnnected(listToDelete);
        }

        private bool IsConnected(GameObject go)
        {
            var status = PrefabUtility.GetPrefabInstanceStatus(go);

            if (status == PrefabInstanceStatus.Connected)
            {
                return true;
            }

            return false;
        }

        private void DeleteConnnected(List<GameObject> gosToDelete)
        {
            while (gosToDelete.Count > 0)
            {
                for (int i = 0; i < gosToDelete.Count; i++)
                {
                    var go = gosToDelete[i];

                    DestroyImmediate(go);

                    if (go == null)
                        gosToDelete.Remove(go);
                }
            }
        }
    }
#endif
}
