using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _GAME.Enemy
{
    public class EnemyFeature : MonoBehaviour
    {
        public EnemySettings EnemySettings;

        public float DelayToStartMoveEnemies;

        public float EnemyFallHeigthToDie = 1.5f;

        public bool IsBossLevel = false;

        [HideInInspector] public int Move;
        [HideInInspector] public int Attack;
        [HideInInspector] public int Fall;
        [HideInInspector] public int Throw;

        public System.Action<EnemyRefs> OnEnemyDie;
        public System.Action<EnemyRefs, bool> OnEnemyDestruct;
        public System.Action<EnemyRefs> OnEnemyDieWithoutDestract;

        public System.Action<EnemyRefs> OnSetDestructablePartsPosition;

        public System.Action OnStartMoveEnemies;

        private async void Awake()
        {
            if (!GameFeature.Instance.IsTestMode)
            {
                var handle = Addressables.LoadAssetAsync<EnemyPrefabsPreset>("EnemyPrefabsPreset");
                await handle.Task;
                EnemySettings.EnemyPrefabsPreset = handle.Result;
                Addressables.Release(handle);
            }
            
            var handle2 = Addressables.LoadAssetAsync<EnemyEffectsPreset>("EnemyEffectsPreset");
            await handle2.Task;
            EnemySettings.EnemyEffectsPreset = handle2.Result;

            Addressables.Release(handle2);


            Move = Animator.StringToHash(EnemySettings.AnimationMoveKey);
            Attack = Animator.StringToHash(EnemySettings.AnimationAttackKey);
            Fall = Animator.StringToHash(EnemySettings.AnimationFallKey);
            Throw = Animator.StringToHash(EnemySettings.AnimationThrowKey);
        }
    }
}
