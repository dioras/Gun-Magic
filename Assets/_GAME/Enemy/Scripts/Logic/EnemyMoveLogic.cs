using UnityEngine;

namespace _GAME.Enemy
{
    public class EnemyMoveLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private Level.LevelFeature _levelFeature;
        private LevelUIView.LevelUIFeature _levelUIFeature;
        private Player.PlayerFeature _playerFeature;
        private VibroService.VibroFeature _vibroFeature;

        private EnemySettings _settings;

        private float _time = 0;
        private float _stopDistance = 0;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _playerFeature = GameFeature.PlayerFeature;
            _vibroFeature = GameFeature.VibroFeature;

            _settings = _enemyFeature.EnemySettings;
            _stopDistance = Mathf.Pow(_settings.StopDistance, 2);
        }

        private void OnEnable()
        {
            _levelFeature.OnStartLevel += StartMoveEnemies;
            _enemyFeature.OnStartMoveEnemies += StartMoveEnemies;
        }

        private void Update()
        {
            if (_levelFeature.Level == null) return;

            //if (_playerFeature.Player == null || !_playerFeature.Player.IsAlive) return;

            foreach (var enemy in _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage)
            {
                if (enemy.IsAlive && enemy.CanMove)
                {
                    //enemy.transform.Translate(enemy.transform.forward * _settings.MoveSpeed * Time.deltaTime, Space.World);

                    _time += Time.deltaTime;
                    if (_time >= 0.1f)
                    {
                        _time = 0;
                        LookAtPlayer(enemy);
                    }

                    //LookAtPlayer(enemy);

                    WaitToStop(enemy);

                    if (!_playerFeature.Player.IsAlive)
                    {
                        StopEnemy(enemy);
                    }
                }
            }
        }

        private void StartMoveEnemies()
        {
            if (_levelFeature.Level.IsTutorial && _playerFeature.CurrentPlatform == 1) return;

            this.DelayedCall(_enemyFeature.DelayToStartMoveEnemies, () =>
             {
                 if (_levelFeature.Level != null)
                 {
                     foreach (var enemy in _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage)
                     {
                         if (!enemy.IsForciblyStay)
                         {
                             if (enemy.Ragdoll)
                             {
                                 enemy.Ragdoll.Animator.SetInteger(_enemyFeature.Move, 1);
                                 enemy.Ragdoll.Animator.applyRootMotion = true;
                             }
                             enemy.CanMove = true;
                         }
                     }
                 }
             });
        }

        private void LookAtPlayer(EnemyRefs enemy)
        {
            if (_playerFeature.Player != null)
            {
                var dir = _playerFeature.Player.transform.position - enemy.transform.position;
                var rotationAngles = Quaternion.LookRotation(dir).eulerAngles;
                var rot = enemy.transform.eulerAngles;
                //var newRot = new Vector3(rot.x, rotationAngles.y, rot.z);

                var newRot = new Vector3(rot.x,
                                         Mathf.LerpAngle(enemy.transform.eulerAngles.y, rotationAngles.y, enemy.RotationSpeed * Time.deltaTime),
                                         rot.z);

                enemy.transform.eulerAngles = newRot;
               
                //if (!Mathf.Approximately(rot.y, newRot.y))
                //{
                //    enemy.transform.eulerAngles = newRot;
                //}
            }
        }

        private void WaitToStop(EnemyRefs enemy)
        {
            if ((enemy.transform.position - _playerFeature.Player.transform.position).sqrMagnitude <= _stopDistance)
            {
                if (_playerFeature.Player.IsAlive)
                {
                    enemy.CanMove = false;
                    _playerFeature.Player.IsAlive = false;
                    _playerFeature.CanShoot = false;

                    enemy.Ragdoll.Animator.applyRootMotion = true;
                    enemy.Ragdoll.Animator.SetInteger(_enemyFeature.Move, 0);
                    enemy.Ragdoll.Animator.SetTrigger(_enemyFeature.Attack);

                    _vibroFeature.OnVibrateLevelFailed?.Invoke();

                    _playerFeature.DeathReason = "Enemy";
                    _levelFeature.OnLevelFailed?.Invoke();
                }
                else
                {
                    StopEnemy(enemy);
                }
            }
        }

        private void StopEnemy(EnemyRefs enemy)
        {
            enemy.CanMove = false;

            enemy.Ragdoll?.Animator?.SetInteger(_enemyFeature.Move, 0);
        }
    }
}
