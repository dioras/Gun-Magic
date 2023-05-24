using _GAME.Audio;
using _GAME.Level;
using UnityEngine;

namespace _GAME.Enemy
{
    public class EnemyCollisionLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private LevelFeature _levelFeature;
        private AudioFeature _audioFeature;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _audioFeature = GameFeature.AudioFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += SubscribeEnemies;
        }

        private void SubscribeEnemies(LevelRefs level)
        {
            foreach (var stage in level.Stages)
            {
                foreach (var enemy in stage.EnemiesOnStage)
                {
                    if (enemy == null)
                        Debug.LogError("Enemy null - in level need click collect all enemies. Level - " + level.gameObject.name);
                    if (enemy.CollisionCatcher)
                        enemy.CollisionCatcher.OnTriggerEnterEvent += (col) => DamageEnemy(col, enemy);
                    if (enemy.SpineCollisionCatcher)
                        enemy.SpineCollisionCatcher.OnTriggerEnterEvent += (col) => DamageEnemy(col, enemy);
                    if (enemy.HeadCollisionCatcher)
                        enemy.HeadCollisionCatcher.OnTriggerEnterEvent += (col) => DamageEnemy(col, enemy);
                }
            }
        }

        private void DamageEnemy(Collider obj, EnemyRefs enemy)
        {
            if (!enemy.IsAlive) return;

            if (1 << enemy.Ragdoll.Colliders[0].gameObject.layer != _enemyFeature.EnemySettings.EnemyLayer) return;

            if (1 << obj.gameObject.layer == _enemyFeature.EnemySettings.DamagableLayer)
            {
                enemy.Ragdoll.ForceDirection = enemy.transform.forward;
                enemy.Ragdoll.ForcePower = 10;
                enemy.Ragdoll.ActivateRagdoll();

                enemy.IsAlive = false;

                _enemyFeature.OnEnemyDie?.Invoke(enemy);
                _enemyFeature.OnEnemyDestruct?.Invoke(enemy, true);
            }

            if (1 << obj.gameObject.layer == _enemyFeature.EnemySettings.DestructableLayer ||
                1 << obj.gameObject.layer == _enemyFeature.EnemySettings.EnemyLayer)
            {
                enemy.Ragdoll.ForceDirection = (enemy.transform.position - obj.transform.position);
                enemy.Ragdoll.ForcePower = 10;
                enemy.Ragdoll.ActivateRagdoll();
                enemy.IsAlive = false;

                _enemyFeature.OnEnemyDie?.Invoke(enemy);
                _enemyFeature.OnEnemyDestruct?.Invoke(enemy, true);
            }
        }
    }
}
