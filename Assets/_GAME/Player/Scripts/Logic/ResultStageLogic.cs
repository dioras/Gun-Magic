using _GAME.Audio;
using _GAME.Enemy;
using _GAME.Level;
using _GAME.LevelUIView;
using _GAME.Tutorial;
using _GAME.VibroService;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace _GAME.Player
{
    public class ResultStageLogic : MonoBehaviour
    {
        private PlayerFeature _playerFeature;
        private EnemyFeature _enemyFeature;
        private LevelFeature _levelFeature;
        private TutorialFeature _tutorial;
        private VibroFeature _vibroFeature;
        private AudioFeature _audioFeature;
        private LevelUIFeature _levelUIFeature;
        private WeaponsFeature _weaponsFeature;

        private Coroutine _moveCoroutine;

        private PlayerRefs _player;

        private void Awake()
        {
            _playerFeature = GameFeature.PlayerFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _tutorial = GameFeature.TutorialFeature;
            _vibroFeature = GameFeature.VibroFeature;
            _audioFeature = GameFeature.AudioFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _weaponsFeature = GameFeature.WeaponsFeature;
        }

        private void OnEnable()
        {
            _enemyFeature.OnEnemyDie += NextStageActivate;
            _levelFeature.OnLevelLoaded += (lvl) =>
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                }

                _playerFeature.CurrentPlatform = 1;
                _playerFeature.DeathReason = "";
                _playerFeature.Player.CollisionCatcher.OnTriggerEnterEvent += StopOnPointCollision;
            };

            _playerFeature.OnPlayerMoveStart += PlayerMoveStart;
            _playerFeature.OnPlayerMoveEnd += PlayerMoveEnd;
        }

        private void NextStageActivate(EnemyRefs enemyRefs)
        {
            int countDie = 0;

            foreach (var enemy in _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage)
            {
                if (!enemy.IsAlive) countDie++;
            }

            if (countDie >= _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage.Count)
            {
                if (_levelFeature.Level.CurrentStageIndex + 1 < _levelFeature.Level.Stages.Length)
                {
                    _levelFeature.Level.CurrentStageIndex++;

                    _moveCoroutine = this.DelayedCall(_playerFeature.DelayToMoveNextStage, () =>
                    {
                        MovePlayer();

                        if (!_levelFeature.Level) return;

                        foreach (var enemy in _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage)
                        {
                            enemy.gameObject.SetActive(true);
                        }
                    }, false);
                }
                else
                {
                    _moveCoroutine = this.DelayedCall(_playerFeature.DelayToMoveNextStage, () =>
                    {
                        MovePlayer();
                    }, false);
                }
            }
        }

        private void StopOnPointCollision(Collider collider)
        {
            if (1 << collider.gameObject.layer == _playerFeature.PlayerSettings.WaypointLayer)
            {
                //if (_levelFeature.Level.IsTutorial && !hasShownBattleTutorial)
                //{
                //    hasShownBattleTutorial = true;
                //    _tutorial.OnShowBattleTutorial(1);
                //}

                if (collider.gameObject.CompareTag("SpeedPoint"))
                {
                    var speedPoint = collider.GetComponent<Env.SpeedPointRefs>();
                    _playerFeature.Player.MoveSpeed = speedPoint.Speed;
                    collider.gameObject.SetActive(false);
                }

                if (_playerFeature.Player.IsCanMove)
                {
                    StopPlayer();

                    collider.gameObject.SetActive(false);

                    _playerFeature.OnCameToNextPlatform?.Invoke(/*TODO: Трекать событие в аналитике */ _playerFeature.CurrentPlatform);

                    _playerFeature.CurrentPlatform++;

                    _enemyFeature.OnStartMoveEnemies?.Invoke();
                }
            }

            if (1 << collider.gameObject.layer == _playerFeature.PlayerSettings.WaypointFinish)
            {
                if (_playerFeature.Player.IsCanMove)
                {
                    _playerFeature.Player.IsCanMove = false;
                    _playerFeature.CanShoot = false;
                    ClambPlayer(collider.transform);

                    _levelFeature.OnLevelComplete?.Invoke();
                }
            }
        }

        private void ClambPlayer(Transform point)
        {
            _player = _playerFeature.Player;
            _playerFeature.IsMoving = false;

            if (!_player) return;

            _weaponsFeature.PlayerWeapon.Animator.SetBool("IsWalking", false);

            //TODO: Jump in transport logic
            /*
            _player.MoveHandsParent.DOLocalMoveY(-0.1f, 0.01f).onComplete += () =>
            {
                if (!_player) return;

                _player.ClambHandsParent.gameObject.SetActive(true);
                _player.MoveHandsParent.gameObject.SetActive(false);

                _player.ClambHandsParent.DOLocalMoveY(0, 0.01f).onComplete += () =>
                {
                    if (!_player) return;

                    _player.ClambHands.enabled = true;

                    _player.transform.SetParent(point);
                    _player.transform.localScale = Vector3.one * 2;
                    DOTween.Sequence()
                    .Join(_player.transform.DOLocalJump(_playerFeature.PlayerSettings.OnHelicopterPosition, .5f, 1,
                                                       _playerFeature.PlayerSettings.JumpToHelicopterDuration))

                    .Join(_player.transform.DOLocalRotate(_playerFeature.PlayerSettings.OnHelicopterRotationAngles,
                                                         _playerFeature.PlayerSettings.JumpToHelicopterDuration));

                    _vibroFeature.OnVibrateMedium?.Invoke();
                    _audioFeature.PlaySound(Audio.EnumSound.ClambHelicopter, volume: 0.7f);

                    this.DelayedCall(0.1f, () => _vibroFeature.OnVibrateMedium?.Invoke());
                };
            };
            */
        }

        private void MovePlayer()
        {
            _playerFeature.OnPlayerMoveStart?.Invoke();

            _player = _playerFeature.Player;
            _playerFeature.CanShoot = false;
            _playerFeature.IsMoving = true;
            _playerFeature.Player.IsImmortal = true;

            if (!_player) return;

            if (_player.IsAlive)
            {
                Time.timeScale = 1;

                _player.IsCanMove = true;

                _weaponsFeature.PlayerWeapon.Animator.SetBool("IsWalking", true);
            }
        }

        private void StopPlayer()
        {
            _playerFeature.OnPlayerMoveEnd?.Invoke();

            _player = _playerFeature.Player;

            _playerFeature.Player.IsCanMove = false;
            _playerFeature.Player.IsImmortal = false;
            _playerFeature.IsMoving = false;

            _playerFeature.CanShoot = true;

            _weaponsFeature.PlayerWeapon.Animator.SetBool("IsWalking", false);
        }

        private void PlayerMoveStart()
        {
            _levelFeature.OnPlatformFinish?.Invoke();
        }

        private void PlayerMoveEnd()
        {
            _levelFeature.PlatformTime = 0;
        }
    }
}
