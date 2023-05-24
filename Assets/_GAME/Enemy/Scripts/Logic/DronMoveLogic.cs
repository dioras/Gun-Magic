using UnityEngine;

namespace _GAME.Enemy
{
    public class DronMoveLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private Level.LevelFeature _levelFeature;
        private LevelUIView.LevelUIFeature _levelUIFeature;
        private Player.PlayerFeature _playerFeature;
        private VibroService.VibroFeature _vibroFeature;

        private DroneRef[] _dronsOnLevel;
        private EnemySettings _settings;
        private float stopDistance = 0;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _playerFeature = GameFeature.PlayerFeature;

            _vibroFeature = GameFeature.VibroFeature;

            _settings = _enemyFeature.EnemySettings;
            stopDistance = Mathf.Pow(_settings.StopDistanceForDrone, 2);
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += (lvl) => _dronsOnLevel = null;
            _levelFeature.OnStartLevel += StartMoveDrons;
            _enemyFeature.OnStartMoveEnemies += StartMoveDrons;
        }

        private void StartMoveDrons()
        {
            _dronsOnLevel = _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].GetComponentsInChildren<DroneRef>();

            this.DelayedCall(_enemyFeature.DelayToStartMoveEnemies, () =>
            {
                if (_dronsOnLevel != null)
                {
                    foreach (var dron in _dronsOnLevel)
                    {
                        dron.IsCanMove = true;
                    }
                }
            });
        }

        private void Update()
        {
            if (_levelFeature.Level == null || _dronsOnLevel == null) return;

            foreach (var dron in _dronsOnLevel)
            {
                if (!dron.IsCanMove || !dron.Enemy.IsAlive) continue;

                dron.transform.Translate(dron.transform.forward *
                                         Time.deltaTime *
                                         _enemyFeature.EnemySettings.MoveSpeed, Space.World);
                LookAtPlayer(dron);

                WaitToStop(dron);

                if (!_playerFeature.Player.IsAlive)
                {
                    StopDron(dron);
                }
            }
        }

        private void StopDron(DroneRef dron)
        {
            dron.IsCanMove = false;
        }

        private void LookAtPlayer(DroneRef dron)
        {
            if (_playerFeature.Player != null)
            {
                var dir = _playerFeature.Player.transform.position - dron.transform.position;
                var rotationAngles = Quaternion.LookRotation(dir).eulerAngles;
                var rot = dron.transform.eulerAngles;
                var newRot = new Vector3(rot.x, rotationAngles.y, rot.z);

                if (!Mathf.Approximately(rot.y, newRot.y))
                {
                    dron.transform.eulerAngles = newRot;
                }
            }
        }

        private void WaitToStop(DroneRef dron)
        {
            var sqrMagnitude = (dron.transform.position - _playerFeature.Player.transform.position).sqrMagnitude;

            if (sqrMagnitude <= stopDistance)
            {
                if (_playerFeature.Player.IsAlive)
                {
                    dron.IsCanMove = false;
                    _playerFeature.Player.IsAlive = false;
                    _playerFeature.CanShoot = false;

                    _vibroFeature.OnVibrateLevelFailed?.Invoke();

                    _playerFeature.DeathReason = "Dron";
                    _levelFeature.OnLevelFailed?.Invoke();
                }
                else
                {
                    StopDron(dron);
                }
            }
        }
    }
}