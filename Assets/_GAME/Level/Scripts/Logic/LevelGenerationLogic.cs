using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _GAME.Level
{
    public class LevelGenerationLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnGenerateLevelFromJSON += BuildLevelFromJSON;
        }

        [ContextMenu("Test read")]
        private void Readtest()
        {
            TaskRun();
        }

        private async void TaskRun()
        {
            await BuildLevelFromJSON("Level 62");
        }

        private async Task BuildLevelFromJSON(string level)
        {
            Physics.autoSimulation = false;

            var textAsset = Resources.Load<TextAsset>($"Levels/{level}");

            string json = textAsset.text;

            var levelObj = JsonConvert.DeserializeObject<LevelJsonObject>(json);

            var levelGo = new GameObject(level);
            var levelRefs = levelGo.AddComponent<LevelRefs>();

            levelRefs.IsTutorial = levelObj.IsTutorial;
            levelRefs.NeedHelicopter = levelObj.NeedHelicopter;

            var finishPos = new Vector3(levelObj.LocalPositionX, levelObj.LocalPositionY, levelObj.LocalPositionZ);
            var finishRot = new Quaternion(levelObj.LocalRotationX, levelObj.LocalRotationY, levelObj.LocalRotationZ, levelObj.LocalRotationW);

            var handle = Addressables.InstantiateAsync(levelObj.LevelFinishPrefabKey, finishPos, finishRot, levelGo.transform);
            await handle.Task;

            var finishPlatform = handle.Result;

            var winCollider = finishPlatform.GetComponent<FinishPlatformRefs>().WinCollider;
            winCollider.transform.localPosition = levelObj.WinColliderLocalPosition.GetPosition();
            winCollider.transform.localRotation = levelObj.WinColliderLocalRotation.GetRotation();

            int stageCount = levelObj.Platforms.Where(pl => pl.PlatformEnemies.Count > 0).Count();

            levelRefs.Stages = new StageRefs[stageCount];

            int i = 0;

            foreach (var pl in levelObj.Platforms)
            {
                var platform = new GameObject(pl.Name);
                platform.transform.SetParent(levelGo.transform);

                foreach (var element in pl.PlatformElements)
                {
                    var pos = new Vector3(element.LocalPositionX, element.LocalPositionY, element.LocalPositionZ);
                    var rot = new Quaternion(element.LocalRotationX, element.LocalRotationY, element.LocalRotationZ, element.LocalRotationW);

                    handle = Addressables.LoadAssetAsync<GameObject>(element.Name);

                    await handle.Task;

                    var platformPrefab = handle.Result;
                    var elGo = Instantiate(platformPrefab, pos, rot, platform.transform);
                }

                if (pl.PlatformEnemies.Count > 0)
                {
                    var stageGo = new GameObject("Stage");
                    stageGo.transform.SetParent(platform.transform);
                    stageGo.transform.localPosition = Vector3.zero;

                    var stage = stageGo.AddComponent<StageRefs>();

                    foreach (var platformEnemy in pl.PlatformEnemies)
                    {
                        var enemyPos = new Vector3(platformEnemy.LocalPositionX, platformEnemy.LocalPositionY, platformEnemy.LocalPositionZ);
                        var enemyRot = new Quaternion(platformEnemy.LocalRotationX, 
                                                      platformEnemy.LocalRotationY, 
                                                      platformEnemy.LocalRotationZ, 
                                                      platformEnemy.LocalRotationW);

                        handle = Addressables.LoadAssetAsync<GameObject>(platformEnemy.Name);
                        await handle.Task;

                        var enemyPrefab = handle.Result;
                        var enemy = Instantiate(enemyPrefab, enemyPos, enemyRot, stage.transform);

                        var enemyRef = enemy.GetComponent<Enemy.EnemyRefs>();
                        enemyRef.IsForciblyStay = platformEnemy.IsForciblyStay;

                        stage.EnemiesOnStage.Add(enemyRef);
                    }

                    stage.EnemiesCount = stage.EnemiesOnStage.Count;
                    levelRefs.Stages[i] = stage;
                    i++;
                }

                platform.transform.localPosition = new Vector3(pl.LocalPositionX,
                                                        pl.LocalPositionY,
                                                        pl.LocalPositionZ);

                platform.transform.localRotation = new Quaternion(pl.LocalRotationX,
                                                          pl.LocalRotationY,
                                                          pl.LocalRotationZ,
                                                          pl.LocalRotationW);
            }

            CreatePath(levelRefs, levelObj);

            _levelFeature.LevelGameObject = levelGo;

            Physics.autoSimulation = true;
        }

        private void CreatePath(LevelRefs levelrefs, LevelJsonObject levelObj)
        {
            var pathGo = new GameObject("Path");
            pathGo.transform.SetParent(levelrefs.gameObject.transform);

            var curve = pathGo.AddComponent<BGCurve>();
            var math = pathGo.AddComponent<BGCcMath>();
            math.Fields = BGCurveBaseMath.Fields.PositionAndTangent;
            math.SectionParts = 10;

            levelrefs.Path = math;

            foreach (var point in levelObj.PathPoints)
            {
                var bgCurvepoint = GetCurvePoint(GetControlType(point.ControlType), curve, point);

                curve.AddPoint(bgCurvepoint);
            }
        }

        private BGCurvePoint.ControlTypeEnum GetControlType(int enumIndex)
        {
            var type = BGCurvePoint.ControlTypeEnum.Absent;

            switch (enumIndex)
            {
                case 2:
                    type = BGCurvePoint.ControlTypeEnum.BezierIndependant;
                    break;
                case 1:
                    type = BGCurvePoint.ControlTypeEnum.BezierSymmetrical;
                    break;
            }

            return type;
        }

        private BGCurvePoint GetCurvePoint(BGCurvePoint.ControlTypeEnum controlType, BGCurve curve, PathPoint point)
        {
            if (controlType == BGCurvePoint.ControlTypeEnum.Absent)
            {
                return new BGCurvePoint(curve, new Vector3(point.PositionWorldX, point.PositionWorldY, point.PositionWorldZ), BGCurvePoint.ControlTypeEnum.Absent);
            }
            else
            {
                return new BGCurvePoint(curve,
                                        new Vector3(point.PositionWorldX, point.PositionWorldY, point.PositionWorldZ),
                                        controlType,
                                        new Vector3(point.ControlFirstWorldX, point.ControlFirstWorldY, point.ControlFirstWorldZ),
                                        new Vector3(point.ControlSecondWorldX, point.ControlSecondWorldY, point.ControlSecondWorldZ));
            }
        }
    }
}
