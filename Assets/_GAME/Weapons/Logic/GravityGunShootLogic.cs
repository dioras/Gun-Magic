using _GAME.Audio;
using _GAME.Common;
using _GAME.Enemy;
using _GAME.Level;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME
{
    public class GravityGunShootLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponsFeature _weaponsFeature;
        private WeaponHubFeature _weaponHubFeature;
        private EnemyFeature _enemyFeature;
        private LevelFeature _levelFeature;
        private AudioFeature _audioFeature;

        private GravityGunRefs _gravityGunRefs;
        private GravityGunSettings _gravityGunSettings;

        private List<GravitySphereRefs> flyingGravitySpheres = new List<GravitySphereRefs>();

        private void Awake()
        {
            _gameFeature = GameFeature.Instance;
            _weaponsFeature = GameFeature.WeaponsFeature;
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _gravityGunSettings = _weaponsFeature.Settings.GravityGunSettings;
            _levelFeature = GameFeature.LevelFeature;
            _audioFeature = GameFeature.AudioFeature;

            _gameFeature.OnTransitionToLevel += InitGravityGun;
            _weaponsFeature.OnShootCurrentWeapon += ShootGravityGun;
            _weaponsFeature.OnGravitySphereHit += HandleGravitySphereHit;
            _levelFeature.OnLevelEnd += ClearFlyingSpheres;
        }

        private void InitGravityGun()
        {
            if (_weaponHubFeature.CraftedWeapon.Type != EnumWeaponType.GravityGun)
                return;

            _gravityGunRefs = _weaponHubFeature.CraftedWeapon.GetComponent<GravityGunRefs>();
        }

        private void ShootGravityGun(Vector3 shootPos)
        {
            if (_weaponsFeature.PlayerWeapon.Type != EnumWeaponType.GravityGun)
                return;

            var gravitySphere = Instantiate(_gravityGunSettings.GravitySpherePrefab, _levelFeature.Level.transform);
            gravitySphere.transform.position = _weaponsFeature.PlayerWeapon.bulletSpawnAnchor.position;
            gravitySphere.transform.rotation = _weaponsFeature.PlayerWeapon.bulletSpawnAnchor.rotation;

            gravitySphere.MoveDirection = (shootPos - gravitySphere.transform.position).normalized;
            gravitySphere.transform.rotation = Quaternion.FromToRotation(Vector3.forward, gravitySphere.MoveDirection);

            gravitySphere.CollisionCatcher.OnTriggerEnterEvent += (collider) => _weaponsFeature.OnGravitySphereHit?.Invoke(gravitySphere, shootPos, collider);

            flyingGravitySpheres.Add(gravitySphere);

            AnimateFakeGravitySphere();

            _audioFeature.PlaySound(EnumSound.GravityGunShoot);
        }

        private void AnimateFakeGravitySphere()
        {
            _gravityGunRefs.GravitySphere.DOKill();
            _gravityGunRefs.GravitySphere.localScale = VectorExtension.FromFloat(0);
            _gravityGunRefs.GravitySphere.DOScale(0.12f, 0.35f).SetDelay(0.15f);
        }

        private void FixedUpdate()
        {
            foreach (var sphere in flyingGravitySpheres)
            {
                sphere.transform.position += _gravityGunSettings.GravitySphereSpeed * Time.fixedDeltaTime * sphere.MoveDirection;
            }
        }

        private void HandleGravitySphereHit(GravitySphereRefs gravitySphere, Vector3 hitPoint, Collider collider)
        {
            ExplodeGravitySphere(gravitySphere);

            this.DelayedCall(0.3f, () => _audioFeature.PlaySound(EnumSound.GravityGunHit, volume: 0.5f));

            foreach (var enemy in _levelFeature.Level.Stages[_levelFeature.Level.CurrentStageIndex].EnemiesOnStage)
            {
                var distanceFromSpehere = Vector3.Distance(enemy.transform.position, gravitySphere.transform.position);

                if (distanceFromSpehere <= _weaponsFeature.Settings.GravityGunSettings.ExplosionRadius)
                {
                    LevitateEnemy(enemy, gravitySphere.transform.position);
                }
            }
        }
        private void ExplodeGravitySphere(GravitySphereRefs gravitySphere)
        {
            flyingGravitySpheres.Remove(gravitySphere);
            gravitySphere.Collider.enabled = false;

            var explosionPos = gravitySphere.transform.position;
            var explosionParticles = Instantiate(_gravityGunSettings.ExplosionParticles, explosionPos, Quaternion.identity, _levelFeature.Level.transform);

            gravitySphere.transform.DOScale(_gravityGunSettings.ExploadingSphereScale, 1f).OnComplete(() =>
            {
                gravitySphere.transform.DOScale(0f, 0.2f).SetDelay(_gravityGunSettings.SphereImploadDelay).OnComplete(() =>
                {
                    gravitySphere.gameObject.SetActive(false);
                    explosionParticles.gameObject.SetActive(false);
                });
                
            });
        }

        private void LevitateEnemy(EnemyRefs enemy, Vector3 blackHolePos)
        {
            int randomAnimationIndex = Random.Range(1, _gravityGunSettings.FloatingAnimationsCount + 1);
            enemy.Ragdoll.Animator.avatar = _enemyFeature.EnemySettings.EnemyPrefabsPreset.EnemyFloatingAvatars[0];
            enemy.Ragdoll.Animator.SetTrigger("Float " + randomAnimationIndex);
            enemy.Ragdoll.Animator.SetFloat("FloatingSpeed", Random.Range(0.65f, 1.2f));

            float levitationHeight = Random.Range(_gravityGunSettings.LevitationMinY, _gravityGunSettings.LevitationMaxY);

            enemy.transform.DORotate(Random.rotation.eulerAngles * 360f, Random.Range(6f, 12f));
            enemy.transform.DOMoveY(enemy.transform.position.AddY(levitationHeight).y, _gravityGunSettings.LevitationDuration).OnComplete(() =>
            {
                enemy.transform.DOKill();

                enemy.transform.DOScale(0f, _gravityGunSettings.IntakeDuration);
                enemy.transform.DOMove(blackHolePos, _gravityGunSettings.IntakeDuration).OnComplete(() =>
                {
                    if (enemy.IsAlive)
                    {
                        enemy.IsAlive = false;

                        _enemyFeature.OnEnemyDie?.Invoke(enemy);
                        _enemyFeature.OnEnemyDestruct?.Invoke(enemy, true);
                    }
                });
            });
        }

        private void ClearFlyingSpheres(bool levelWon)
        {
            flyingGravitySpheres.Clear();
        }
    }
}