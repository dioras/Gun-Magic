using UnityEngine;
using BansheeGz.BGSpline.Components;
using System.Linq;
using UnityEngine.AI;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace _GAME.Level
{
    public class LevelRefs : MonoBehaviour
    {
        public bool IsStarted = false;

        public StageRefs[] Stages;

        public int CurrentStageIndex;

        public bool IsTutorial = false;

        public int tutorialIndex;

        public bool NeedHelicopter = true;

        public BGCcMath Path;

        public NavMeshSurface[] NavMeshSurfaces;

        public Enemy.EnemyRefs[] EnemyPrefabs;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelRefs))]
    public class LevelRefsEditor : Editor
    {
        private LevelRefs _level;

        private readonly string _pathToSaveJson = "Assets/Resources/Levels/";
        public override void OnInspectorGUI()
        {
            _level = (LevelRefs)target;
            base.OnInspectorGUI();

            if (GUILayout.Button("TEST level"))
            {
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                SaveToJSON();

                var levelFeature = FindObjectOfType<Level.LevelFeature>();
                levelFeature.LevelCollection.IsTestMode = true;

                GameObject lvlPrefab = null;

                string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/_GAME/Level/Prefabs" });

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path);

                    if (fileName == _level.gameObject.name)
                    {
                        lvlPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                    }
                }


                levelFeature.LevelCollection.TestLevelKey = lvlPrefab.name;

                EditorApplication.EnterPlaymode();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Path"))
            {
                CreatePath();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Collect all enemies on level"))
            {
                CollectAllEnemiesOnLevel();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Clear any enemies"))
            {
                ClearAnyEnemies();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Switch all enemies"))
            {
                foreach (var st in _level.Stages)
                {
                    foreach (var enemy in st.EnemiesOnStage)
                    {
                        var newEnemy = PrefabUtility.InstantiatePrefab(_level.EnemyPrefabs[0]) as Enemy.EnemyRefs;

                        newEnemy.transform.rotation = enemy.transform.rotation;
                        newEnemy.transform.position = enemy.transform.position;
                        newEnemy.transform.SetParent(enemy.transform.parent);
                        enemy.gameObject.SetActive(false);
                    }

                    st.EnemiesOnStage = st.gameObject.GetComponentsInChildren<Enemy.EnemyRefs>(false).ToList();

                    st.EnemiesCount = st.EnemiesOnStage.Count;
                }

                EditorUtility.SetDirty(_level);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Switch random enemies"))
            {
                foreach (var st in _level.Stages)
                {
                    var enemy = st.EnemiesOnStage[Random.Range(0, st.EnemiesOnStage.Count)];

                    var newEnemy = PrefabUtility.InstantiatePrefab(_level.EnemyPrefabs[0]) as Enemy.EnemyRefs;

                    newEnemy.transform.rotation = enemy.transform.rotation;
                    newEnemy.transform.position = enemy.transform.position;
                    newEnemy.transform.SetParent(enemy.transform.parent);
                    enemy.gameObject.SetActive(false);

                    st.EnemiesOnStage = st.gameObject.GetComponentsInChildren<Enemy.EnemyRefs>(false).ToList();

                    st.EnemiesCount = st.EnemiesOnStage.Count;
                }

                //EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Switch all any enemies"))
            {
                foreach (var st in _level.Stages)
                {
                    for (int i = 0; i < _level.EnemyPrefabs.Length; i++)
                    {
                        foreach (var enemy in st.EnemiesOnStage)
                        {
                            //if (enemy.IsGrenadeThrower) continue;

                            if (enemy.gameObject.name.Contains(_level.EnemyPrefabs[i].name))
                            {
                                var newEnemy = PrefabUtility.InstantiatePrefab(_level.EnemyPrefabs[i]) as Enemy.EnemyRefs;

                                newEnemy.transform.rotation = enemy.transform.rotation;
                                newEnemy.transform.position = enemy.transform.position;
                                newEnemy.transform.SetParent(enemy.transform.parent);
                                enemy.gameObject.SetActive(false);
                            }
                        }
                    }

                    st.EnemiesOnStage = st.gameObject.GetComponentsInChildren<Enemy.EnemyRefs>(false).ToList();

                    st.EnemiesCount = st.EnemiesOnStage.Count;
                }

                EditorUtility.SetDirty(_level);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Optimize Level"))
            {
                for (int i = 1; i < _level.Stages.Length; i++)
                {
                    foreach (var enemy in _level.Stages[i].EnemiesOnStage)
                    {
                        enemy.gameObject.SetActive(false);
                    }
                }

                var platforms = _level.gameObject.GetComponentsInChildren<Transform>(true).Where(go => go.name.Contains("Platf")).ToArray();

                foreach (var plat in platforms)
                {
                    var notActiveObjects = plat.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).Where(g => g.gameObject.activeInHierarchy == false).ToArray();

                    foreach (var obj in notActiveObjects)
                    {
                        if (obj != null)
                        {
                            try
                            {
                                DestroyImmediate(obj);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }

                EditorUtility.SetDirty(_level);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Level to Json"))
            {
                SaveToJSON();
            }
        }
        private void SaveToJSON()
        {
            var levelJsonObj = new LevelJsonObject();

            levelJsonObj.IsTutorial = _level.IsTutorial;
            levelJsonObj.NeedHelicopter = _level.NeedHelicopter;

            foreach (Transform child in _level.transform)
            {
                if (child.gameObject.name.Contains("Finish"))
                {
                    levelJsonObj.LevelFinishPrefabKey = child.gameObject.name;
                    levelJsonObj.LocalPositionX = child.transform.localPosition.x;
                    levelJsonObj.LocalPositionY = child.transform.localPosition.y;
                    levelJsonObj.LocalPositionZ = child.transform.localPosition.z;

                    levelJsonObj.LocalRotationX = child.transform.localRotation.x;
                    levelJsonObj.LocalRotationY = child.transform.localRotation.y;
                    levelJsonObj.LocalRotationZ = child.transform.localRotation.z;
                    levelJsonObj.LocalRotationW = child.transform.localRotation.w;

                    var winCollider = child.GetComponent<FinishPlatformRefs>().WinCollider;

                    if (winCollider)
                    {
                        levelJsonObj.WinColliderLocalPosition = new CustomPosition(winCollider.transform.localPosition);
                        levelJsonObj.WinColliderLocalRotation = new CustomRotation(winCollider.transform.localRotation);
                    }

                    continue;
                }
                else
                {
                    if (child.gameObject.name.Contains("Platf"))
                    {
                        if (!child.gameObject.activeSelf) continue;

                        var platform = new Platform
                        {
                            Name = child.gameObject.name,
                            LocalPositionX = child.localPosition.x,
                            LocalPositionY = child.localPosition.y,
                            LocalPositionZ = child.localPosition.z,

                            LocalRotationX = child.localRotation.x,
                            LocalRotationY = child.localRotation.y,
                            LocalRotationZ = child.localRotation.z,
                            LocalRotationW = child.localRotation.w,
                        };

                        foreach (Transform prefab in child.transform)
                        {
                            if (IsConnected(prefab.gameObject))
                            {
                                if (!prefab.gameObject.activeSelf) continue;

                                platform.PlatformElements.Add(new PlatformPrefab
                                {
                                    Name = PrefabUtility.GetCorrespondingObjectFromSource(prefab.gameObject).name,   //prefab.gameObject.name,
                                    LocalPositionX = prefab.localPosition.x,
                                    LocalPositionY = prefab.localPosition.y,
                                    LocalPositionZ = prefab.localPosition.z,

                                    LocalRotationX = prefab.localRotation.x,
                                    LocalRotationY = prefab.localRotation.y,
                                    LocalRotationZ = prefab.localRotation.z,
                                    LocalRotationW = prefab.localRotation.w,
                                });
                            }
                        }

                        var stage = child.GetComponentInChildren<StageRefs>();

                        if (stage)
                        {
                            foreach (var enemy in stage.EnemiesOnStage)
                            {
                                if (!enemy.gameObject.activeSelf) continue;

                                platform.PlatformEnemies.Add(new PlatformPrefab
                                {
                                    Name = PrefabUtility.GetCorrespondingObjectFromSource(enemy.gameObject).name,
                                    LocalPositionX = enemy.transform.localPosition.x,
                                    LocalPositionY = enemy.transform.localPosition.y,
                                    LocalPositionZ = enemy.transform.localPosition.z,

                                    LocalRotationX = enemy.transform.localRotation.x,
                                    LocalRotationY = enemy.transform.localRotation.y,
                                    LocalRotationZ = enemy.transform.localRotation.z,
                                    LocalRotationW = enemy.transform.localRotation.w,

                                    IsForciblyStay = enemy.IsForciblyStay
                                });
                            }
                        }

                        levelJsonObj.Platforms.Add(platform);
                    }
                }
            }

            var curve = _level.Path.GetComponent<BGCurve>();

            foreach (var point in curve.Points)
            {
                levelJsonObj.PathPoints.Add(new PathPoint(point));
            }

            var jsonStr = JsonConvert.SerializeObject(levelJsonObj);

            File.WriteAllText(_pathToSaveJson + $"{_level.gameObject.name}.json", jsonStr);
        }
        private void CollectAllEnemiesOnLevel()
        {
            var listStage = new List<StageRefs>();

            foreach (Transform child in _level.transform)
            {
                if (child.gameObject.name.Contains("Platf") && child.gameObject.activeSelf)
                {
                    var stage = child.gameObject.GetComponentInChildren<StageRefs>();
                    if (stage) listStage.Add(stage);
                }
            }

            _level.Stages = new StageRefs[listStage.Count];
            _level.Stages = listStage.ToArray();

            foreach (var st in _level.Stages)
            {
                st.EnemiesOnStage = st.gameObject.GetComponentsInChildren<Enemy.EnemyRefs>(false).ToList();

                st.EnemiesCount = st.EnemiesOnStage.Count;

                EditorUtility.SetDirty(st);
            }

            EditorUtility.SetDirty(_level);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void ClearAnyEnemies()
        {
            foreach (var stage in _level.Stages)
            {
                if (stage.EnemiesOnStage.Count == 5)
                {
                    RemoveEnemy(2, stage);
                    //EditorUtility.SetDirty(level);
                    //AssetDatabase.RenameAsset(currentObjectPrefabPath, creatureStats.creatureName);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    continue;
                }

                if (stage.EnemiesOnStage.Count == 4)
                {
                    RemoveEnemy(1, stage);
                    //EditorUtility.SetDirty(level);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    continue;
                }

                if (stage.EnemiesOnStage.Count == 3)
                {
                    RemoveEnemy(1, stage);
                    //EditorUtility.SetDirty(level);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    continue;
                }
            }
        }
        private void RemoveEnemy(int count, StageRefs stage)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < stage.EnemiesOnStage.Count; j++)
                {
                    var enemy = stage.EnemiesOnStage[j];

                    if (enemy.gameObject.name.Contains("_Robot_lp"))
                    {
                        stage.EnemiesOnStage.Remove(enemy);

                        DestroyImmediate(enemy.gameObject);
                    }
                }
            }
        }
        private void CreatePath()
        {
            var platforms = new List<Transform>();

            foreach (Transform child in _level.transform)
            {
                if (child.gameObject.name.Contains("Platf") && child.gameObject.activeSelf) platforms.Add(child);
            }

            var finish = platforms[0];
            platforms.Remove(finish);
            platforms.Add(finish);

            BuildPath(platforms);
        }
        private void BuildPath(List<Transform> platforms)
        {
            if (_level.Path)
            {
                do DestroyImmediate(_level.Path.gameObject);
                while (_level.Path != null);
            }

            //add spline
            var newPath = new GameObject("Path");
            newPath.transform.SetParent(_level.transform);
            var curve = newPath.gameObject.AddComponent<BGCurve>();

            //add points
            for (int i = 0; i < platforms.Count; i++)
            {
                var pl = platforms[i];
                var pos = pl.transform.position;
                var heightVector = Vector3.zero;
                var distance = 0f;
                var platformWidth = 16;

                var newHeigth = 0f;
                var newWidth = 0f;

                if ((i + 1) < platforms.Count)
                {
                    var nextPosition = platforms[i + 1].transform.position;
                    distance = nextPosition.z - pl.transform.position.z;

                    if (nextPosition.y > pl.transform.position.y)
                    {
                        platformWidth = 8;
                        newHeigth = (nextPosition.y - pl.transform.position.y) * 1.5f;
                        newWidth = 1;
                    }
                    else
                    {
                        platformWidth = 16;
                        newHeigth = distance / 4;
                        newWidth = distance / 4;
                    }

                    if ((nextPosition.z - pl.transform.position.z) > 20 && (nextPosition.y > pl.transform.position.y))
                    {
                        platformWidth = 16;
                        newHeigth = distance / 4;
                        newWidth = distance / 4;
                    }
                }

                heightVector.y = newHeigth;
                heightVector.z = newWidth;

                curve.AddPoint(new BGCurvePoint(curve, pos, BGCurvePoint.ControlTypeEnum.Absent));

                pos.z += platformWidth;

                curve.AddPoint(new BGCurvePoint(curve, pos, BGCurvePoint.ControlTypeEnum.BezierIndependant,
                   Vector3.zero, heightVector));
            }

            //add math solver
            var math = newPath.AddComponent<BGCcMath>();
            math.Fields = BGCurveBaseMath.Fields.PositionAndTangent;
            math.SectionParts = 10;

            //print calculated values
            Debug.Log("Spline's Length=" + math.GetDistance());
            Debug.Log("Position at the middle=" + math.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, .5f));
            Debug.Log("Point, closest to Vector3.one, =" + math.CalcPositionByClosestPoint(Vector3.one));

            _level.Path = math;
            EditorUtility.SetDirty(_level);
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
    }
#endif


    [System.Serializable]
    public class LevelJsonObject
    {
        public bool IsTutorial = false;
        public bool NeedHelicopter = true;

        public string LevelFinishPrefabKey;

        public float LocalPositionX;
        public float LocalPositionY;
        public float LocalPositionZ;

        public float LocalRotationX;
        public float LocalRotationY;
        public float LocalRotationZ;
        public float LocalRotationW;

        public CustomPosition WinColliderLocalPosition;
        public CustomRotation WinColliderLocalRotation;

        public List<Platform> Platforms = new List<Platform>();

        public List<PathPoint> PathPoints = new List<PathPoint>();

        public List<SpiderPath> SpiderPaths = new List<SpiderPath>();
    }

    [System.Serializable]
    public class CustomPosition
    {
        public float LocalPositionX;
        public float LocalPositionY;
        public float LocalPositionZ;

        public CustomPosition(Vector3 posiiton)
        {
            LocalPositionX = posiiton.x;
            LocalPositionY = posiiton.y;
            LocalPositionZ = posiiton.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(LocalPositionX, LocalPositionY, LocalPositionZ);
        }
    }

    [System.Serializable]
    public class CustomRotation
    {
        public float LocalRotationX;
        public float LocalRotationY;
        public float LocalRotationZ;
        public float LocalRotationW;

        public CustomRotation(Quaternion rotation)
        {
            LocalRotationX = rotation.x;
            LocalRotationY = rotation.y;
            LocalRotationZ = rotation.z;
            LocalRotationW = rotation.w;
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(LocalRotationX, LocalRotationY, LocalRotationZ, LocalRotationW);
        }
    }


    [System.Serializable]
    public class Platform
    {
        public string Name;

        public float LocalPositionX;
        public float LocalPositionY;
        public float LocalPositionZ;

        public float LocalRotationX;
        public float LocalRotationY;
        public float LocalRotationZ;
        public float LocalRotationW;

        public StageRefs Stage;
        public List<PlatformPrefab> PlatformElements = new List<PlatformPrefab>();
        public List<PlatformPrefab> PlatformEnemies = new List<PlatformPrefab>();
    }

    [System.Serializable]
    public class PlatformPrefab
    {
        public string Name;

        public bool IsForciblyStay = false;

        public float LocalPositionX;
        public float LocalPositionY;
        public float LocalPositionZ;

        public float LocalRotationX;
        public float LocalRotationY;
        public float LocalRotationZ;
        public float LocalRotationW;
    }

    [System.Serializable]
    public class PathPoint
    {
        public int ControlType;

        public float PositionWorldX;
        public float PositionWorldY;
        public float PositionWorldZ;

        public float ControlFirstWorldX;
        public float ControlFirstWorldY;
        public float ControlFirstWorldZ;

        public float ControlSecondWorldX;
        public float ControlSecondWorldY;
        public float ControlSecondWorldZ;

        public PathPoint() { }
        public PathPoint(BGCurvePointI point, bool uselocalpoints = true)
        {
            if (uselocalpoints)
            {
                PositionWorldX = point.PositionLocal.x;
                PositionWorldY = point.PositionLocal.y;
                PositionWorldZ = point.PositionLocal.z;

                ControlFirstWorldX = point.ControlFirstLocal.x;
                ControlFirstWorldY = point.ControlFirstLocal.y;
                ControlFirstWorldZ = point.ControlFirstLocal.z;

                ControlSecondWorldX = point.ControlSecondLocal.x;
                ControlSecondWorldY = point.ControlSecondLocal.y;
                ControlSecondWorldZ = point.ControlSecondLocal.z;
            }
            else
            {
                PositionWorldX = point.PositionWorld.x;
                PositionWorldY = point.PositionWorld.y;
                PositionWorldZ = point.PositionWorld.z;

                ControlFirstWorldX = point.ControlFirstWorld.x;
                ControlFirstWorldY = point.ControlFirstWorld.y;
                ControlFirstWorldZ = point.ControlFirstWorld.z;

                ControlSecondWorldX = point.ControlSecondWorld.x;
                ControlSecondWorldY = point.ControlSecondWorld.y;
                ControlSecondWorldZ = point.ControlSecondWorld.z;
            }

            ControlType = (int)point.ControlType;
        }
    }


    [System.Serializable]
    public class SpiderPath
    {
        public List<PathPoint> Points = new List<PathPoint>();
    }
}
