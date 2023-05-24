using UnityEngine;

namespace _GAME.Enemy
{
    public class DestructDroneLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private Level.LevelFeature _levelFeature;
        private VibroService.VibroFeature _vibroFeature;
        private Audio.AudioFeature _audioFeature;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _vibroFeature = GameFeature.VibroFeature;
            _audioFeature = GameFeature.AudioFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += SubscribeDroneCollision;
        }

        private void SubscribeDroneCollision(Level.LevelRefs level)
        {
            var drons = level.GetComponentsInChildren<DroneRef>(true);
            foreach (var drone in drons)
            {
                drone.CollisionCatcher.OnTriggerEnterEvent += (col) => DesctructDrone(col, drone);
            }
        }

        private void DesctructDrone(Collider col, DroneRef drone)
        {
            if (/*(1 << col.gameObject.layer == _enemyFeature.EnemySettings.DamagableLayer) && */!drone.IsTriggered)
            {
                float delay = drone.DelayToDestroy;

                drone.IsTriggered = true;

                //drone.DamageEffect.gameObject.SetActive(true);
                //drone.DamageEffect.transform.SetParent(_levelFeature.Level.transform);

                drone.DestructedPartsParent.gameObject.SetActive(true);
                drone.DestructedPartsParent.SetParent(_levelFeature.Level.transform);

                drone.Model.SetActive(false);
                drone.gameObject.SetActive(false);

                var effect = Instantiate(_enemyFeature.EnemySettings.EnemyEffectsPreset.BoomEnemyEffect);
                effect.transform.position = drone.transform.position;

                _vibroFeature.OnVibrateMedium?.Invoke();
                _audioFeature.PlaySound(Audio.EnumSound.BarrelExplosion, volume: 0.5f);

                foreach (var rb in drone.DestructedParts)
                {
                    rb.rotation = Random.rotation;
                    rb.AddForce(rb.transform.forward * drone.ExplosionForce, ForceMode.Impulse);
                }

                drone.Enemy.IsAlive = false;
                _enemyFeature.OnEnemyDie?.Invoke(drone.Enemy);

                Destroy(drone.DestructedPartsParent.gameObject, delay);
            }
        }
    }
}
