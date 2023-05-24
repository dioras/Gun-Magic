using _GAME.Audio;
using _GAME.Common;
using _GAME.Enemy;
using _GAME.Level;
using _GAME.Player;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace _GAME
{
    public class BalloonCrossbowShootLogic : MonoBehaviour
    {
        private WeaponsFeature _weaponsFeature;
        private EnemyFeature _enemyFeature;
        private PlayerFeature _playerFeature;
        private LevelFeature _levelFeature;
        private AudioFeature _audioFeature;

        private BalloonCrossbowSettings _crossbowSettings;

        private List<ArrowRefs> flyingArrows = new List<ArrowRefs>();

        private int enemyCryIndex = 1;

        private void Awake()
        {
            _weaponsFeature = GameFeature.WeaponsFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _crossbowSettings = _weaponsFeature.Settings.BalloonCrossbowSettings;
            _playerFeature = GameFeature.PlayerFeature;
            _levelFeature = GameFeature.LevelFeature;
            _audioFeature = GameFeature.AudioFeature;

            _weaponsFeature.OnShootCurrentWeapon += ShootBalloonCrossbow;
            _weaponsFeature.OnArrowHit += ProcessArrowHit;
            _levelFeature.OnLevelEnd += ClearFlyingArrows;
        }

        private void ShootBalloonCrossbow(Vector3 shotPos)
        {
            if (_weaponsFeature.PlayerWeapon.Type != EnumWeaponType.BaloonCrossbow)
                return;

            var arrow = Instantiate(_crossbowSettings.ArrowPrefab, _levelFeature.Level.transform);
            arrow.transform.position = _weaponsFeature.PlayerWeapon.bulletSpawnAnchor.position;
            arrow.transform.localScale = VectorExtension.FromFloat(_crossbowSettings.LaunchedArrowScale);

            arrow.MoveDirection = (shotPos - arrow.transform.position).normalized;
            arrow.transform.rotation = Quaternion.FromToRotation(Vector3.forward, arrow.MoveDirection);

            arrow.CollisionCatcher.OnTriggerEnterEvent += (collider) => _weaponsFeature.OnArrowHit?.Invoke(arrow, shotPos, collider);

            flyingArrows.Add(arrow);

            _audioFeature.PlaySound(EnumSound.BalloonCrossbowShoot);
        }

        private void FixedUpdate()
        {
            foreach (var arrow in flyingArrows)
            {
                arrow.transform.position += _crossbowSettings.ArrowSpeed * Time.fixedDeltaTime * arrow.MoveDirection;
            }
        }
        private void ProcessArrowHit(ArrowRefs arrow, Vector3 hitPoint, Collider collider)
        {
            flyingArrows.Remove(arrow);
            arrow.Collider.enabled = false;

            _audioFeature.PlaySound(EnumSound.BalloonCrossbowHit);

            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                arrow.gameObject.SetActive(false);

                var enemy = collider.GetComponentInParent<EnemyRefs>();

                var closestEnemyForcePart = enemy.AddforceRigibodies.MinBy(r => Vector3.Distance(hitPoint, r.Rigidbody.transform.position));

                var flyingBalloon = Instantiate(_crossbowSettings.FlyingBalloonPrefab, _levelFeature.Level.transform);
                flyingBalloon.transform.position = hitPoint;

                var finalScale = flyingBalloon.transform.localScale.x;
                flyingBalloon.transform.localScale = VectorExtension.FromFloat(0.05f);
                flyingBalloon.transform.DOMove(closestEnemyForcePart.Rigidbody.transform.position, 0.15f);
                flyingBalloon.transform.DOScale(VectorExtension.FromFloat(finalScale), 0.3f).OnComplete(() =>
                {
                    HookEnemy(enemy, closestEnemyForcePart.Rigidbody, flyingBalloon);
                });

                enemy.Ragdoll.Animator.enabled = false;
                enemy.Ragdoll.ActivateRagdoll();
            }
            else if (collider.CompareTag("Barrel"))
            {
                Destroy(arrow.gameObject);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                arrow.transform.SetParent(collider.transform);
        }

        private void HookEnemy(EnemyRefs enemy, Rigidbody hookedBodypart, FlyingBalloonRefs flyingBalloon)
        {
            hookedBodypart.transform.position = flyingBalloon.transform.position;

            flyingBalloon.Joint.connectedBody = hookedBodypart;

            var balloonDestination = RandomHelper.RandomElement(_playerFeature.Player.BalloonPossibleTrajectories).position;

            flyingBalloon.transform.DOMove(balloonDestination, 5f);

            PlayFloatingEnemySound();

            this.DelayedCall(1.8f, () =>
            {
                enemy.IsAlive = false;

                _enemyFeature.OnEnemyDie?.Invoke(enemy);
                _enemyFeature.OnEnemyDestruct?.Invoke(enemy, true);
            });
        }

        private void PlayFloatingEnemySound()
        {
            switch (enemyCryIndex)
            {
                case 1: _audioFeature.PlaySound(EnumSound.FloatingEnemy1); break;
                case 2: _audioFeature.PlaySound(EnumSound.FloatingEnemy2); break;
                case 3: _audioFeature.PlaySound(EnumSound.FloatingEnemy3); break;
                case 4: _audioFeature.PlaySound(EnumSound.FloatingEnemy4); break;
                case 5: _audioFeature.PlaySound(EnumSound.FloatingEnemy5); break;
            }

            enemyCryIndex++;
            if (enemyCryIndex > 5)
                enemyCryIndex = 1;
        }

        private void ClearFlyingArrows(bool levelWon)
        {
            flyingArrows.Clear();
        }
    }
}