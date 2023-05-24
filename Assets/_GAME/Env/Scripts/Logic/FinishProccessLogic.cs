using _GAME.Level;
using UnityEngine;

namespace _GAME.Env
{
    public class FinishProccessLogic : MonoBehaviour
    {
        private EnvFeature _envFeature;
        private Level.LevelFeature _levelFeature;

        private void Awake()
        {
            _envFeature = FindObjectOfType<EnvFeature>();
            _levelFeature = GameFeature.LevelFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += SubscribeFinishPoint;
        }

        private void SubscribeFinishPoint(LevelRefs level)
        {
            var finishPoint = level.GetComponentInChildren<FinishPointRefs>();
            
            if (finishPoint)
            {
                finishPoint.CollisionCatcher.OnTriggerEnterEvent += (col) => ConfettiPlay(col, finishPoint);
            }
        }

        private void ConfettiPlay(Collider obj, FinishPointRefs finishPoint)
        {
            if (1 << obj.gameObject.layer == LayerMask.NameToLayer("Player"));
            {
                finishPoint.CollisionCatcher.enabled = false;
                
                foreach (var effect in finishPoint.Effects)
                {
                    effect.gameObject.SetActive(true);
                } 
            }
        }
    }
}
