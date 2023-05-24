using _GAME.Audio;
using _GAME.Enemy;
using _GAME.Level;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME
{
    public class FrostKeeperShootLogic : MonoBehaviour
    {
        private WeaponsFeature _weaponsFeature;
        private EnemyFeature _enemyFeature;
        private LevelFeature _levelFeature;
        private AudioFeature _audioFeature;

        private FrostKeeperSettings _frostKeeperSettings;

        private List<IceSphereRefs> flyingIceSpheres = new List<IceSphereRefs>();

        private void Awake()
        {
            _weaponsFeature = GameFeature.WeaponsFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _frostKeeperSettings = _weaponsFeature.Settings.FrostKeeperSettings;
            _levelFeature = GameFeature.LevelFeature;
            _audioFeature = GameFeature.AudioFeature;

            _weaponsFeature.OnShootCurrentWeapon += ShootFrostKeeper;
            _weaponsFeature.OnIceSphereHit += HandleIceSphereHit;
            _levelFeature.OnLevelEnd += ClearFlyingSpheres;
        }

        private void ShootFrostKeeper(Vector3 shootPos)
        {
            if (_weaponsFeature.PlayerWeapon.Type != EnumWeaponType.FrostKeeper)
                return;

            var iceSphere = Instantiate(_frostKeeperSettings.IceSpherePrefab, _levelFeature.Level.transform);
            iceSphere.transform.position = _weaponsFeature.PlayerWeapon.bulletSpawnAnchor.position;

            iceSphere.MoveDirection = (shootPos - iceSphere.transform.position).normalized;
            iceSphere.transform.rotation = Quaternion.FromToRotation(Vector3.forward, iceSphere.MoveDirection);

            iceSphere.CollisionCatcher.OnTriggerEnterEvent += (collider) => _weaponsFeature.OnIceSphereHit?.Invoke(iceSphere, shootPos, collider);

            flyingIceSpheres.Add(iceSphere);

            _audioFeature.PlaySound(EnumSound.FrostKeeperShoot);
        }

        private void FixedUpdate()
        {
            foreach (var sphere in flyingIceSpheres)
            {
                sphere.transform.position += _frostKeeperSettings.IceSphereSpeed * Time.fixedDeltaTime * sphere.MoveDirection;
            }
        }

        private void HandleIceSphereHit(IceSphereRefs iceSphere, Vector3 hitPoint, Collider collider)
        {
            ExplodeIceSphere(iceSphere);
            _audioFeature.PlaySound(EnumSound.FrostKeeperHit);

            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemy = collider.GetComponentInParent<EnemyRefs>();
                FreezeEnemy(enemy);
            }
        }

        private void ExplodeIceSphere(IceSphereRefs iceSphere)
        {
            flyingIceSpheres.Remove(iceSphere);
            iceSphere.gameObject.SetActive(false);

            var explosionPos = iceSphere.transform.position;
            Instantiate(_weaponsFeature.Settings.FrostKeeperSettings.SphereExplosionPrefab, explosionPos, Quaternion.identity, _levelFeature.Level.transform);
        }

        private void FreezeEnemy(EnemyRefs enemy)
        {
            enemy.Ragdoll.Animator.enabled = false;

            enemy.FreezeEffect.Ice.transform.localScale = VectorExtension.FromFloat(0.1f);
            enemy.FreezeEffect.Ice.gameObject.SetActive(true);
            enemy.FreezeEffect.Ice.transform.DOScale(1f, 0.2f);

            this.DelayedCall(0.6f, () =>
            {
                ExplodeFrozenEnemy(enemy);
            });
        }

        private void ExplodeFrozenEnemy(EnemyRefs enemy)
        {
            enemy.IsAlive = false;

            _enemyFeature.OnEnemyDie?.Invoke(enemy);
            _enemyFeature.OnEnemyDestruct?.Invoke(enemy, true);

            enemy.FreezeEffect.Ice.gameObject.SetActive(false);

            var shatteredIceContainer = enemy.FreezeEffect.ShatteredPieces[0].transform.parent;
            shatteredIceContainer.SetParent(_levelFeature.Level.transform);

            foreach (var icePiece in enemy.FreezeEffect.ShatteredPieces)
            {
                icePiece.gameObject.SetActive(true);

                icePiece.gameObject.layer = LayerMask.NameToLayer("Ignored");

                var randomForceVector = Random.insideUnitSphere.AddY(0.2f) * _weaponsFeature.Settings.FrostKeeperSettings.IceExplosionStrenght;
                icePiece.AddForce(randomForceVector, ForceMode.Impulse);
            }

            _audioFeature.PlaySound(EnumSound.ShatterEnemy, volume: 0.3f);
        }
        private void ClearFlyingSpheres(bool levelWon)
        {
            flyingIceSpheres.Clear();
        }
    }
}