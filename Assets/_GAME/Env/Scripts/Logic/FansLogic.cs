using _GAME.Level;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Env
{
    public class FansLogic : MonoBehaviour
    {
        private Level.LevelFeature _levelFeature;
        private Enemy.EnemyFeature _enemyFeature;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
            _enemyFeature = GameFeature.EnemyFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += InitFans;
        }

        private void InitFans(LevelRefs level)
        {
            var fans = level.GetComponentsInChildren<FanRefs>();

            foreach (var fan in fans)
            {
                fan.CollisionCatcher.OnTriggerEnterEvent += (col) => FanAction(col, fan);
            }
        }

        private void FanAction(Collider col, FanRefs fan)
        {

        }

        private void MoveHook(Transform hook, DOTweenPath path, System.Action onComplete)
        {
            hook.DOLocalPath(path.wps.ToArray(), path.duration, path.pathType, path.pathMode).onComplete += () =>
            {
                onComplete?.Invoke();
            };
        }
    }
}
