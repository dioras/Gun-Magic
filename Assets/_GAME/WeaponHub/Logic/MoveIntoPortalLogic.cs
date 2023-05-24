using UnityEngine;
using DG.Tweening;
using _GAME.Level;

namespace _GAME
{
    public class MoveIntoPortalLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponHubFeature _weaponHubFeature;
        private WeaponsFeature _weaponsFeature;

        private Vector3 _startTablePos;
        private Quaternion _startTableRot;
        private void OnEnable()
        {
            _gameFeature = GameFeature.Instance;
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _weaponsFeature = GameFeature.WeaponsFeature;

            _weaponHubFeature.OnMoveIntoPortal += StartMovingToPortal;
            _weaponHubFeature.OnReloadHub += RestoreTablePos;
        }

        private void StartMovingToPortal()
        {
            var cameraTransform = _weaponHubFeature.Refs.HubCamera.transform;

            cameraTransform.DOLookAt(_weaponHubFeature.Refs.CameraLookAtAnchor.position, 5f);
            cameraTransform.DOMove(_weaponHubFeature.Refs.CameraMoveAnchor.position, 5f);

            var monitor = _weaponHubFeature.Refs.MonitorTransform;
            monitor.DOMove(monitor.transform.position.AddY(5f), 3f);

            var table = _weaponHubFeature.Refs.TableTransform;
            _startTablePos = table.position;
            _startTableRot = table.rotation;
            table.DOLocalRotate(Vector3.zero.AddX(5f), 3.2f);
            table.DOLocalMoveY(-1f, 2.5f);

            this.DelayedCall(1.15f, PickupWeapon);
            this.DelayedCall(2f, StartTransitionToLevel);
        }

        private void PickupWeapon()
        {
            var weapon = _weaponHubFeature.CraftedWeapon.transform;
            weapon.DOKill();

            weapon.SetParent(_weaponHubFeature.Refs.HubCamera.transform);

            weapon.DOLocalMove(_weaponHubFeature.CraftedWeapon.EquippedPosition.AddZ(0.49f), 0.7f);
            weapon.DOLocalRotate(_weaponHubFeature.CraftedWeapon.EquippedRotation.AddX(-9.8f).AddY(-2.4f), 0.7f);
            weapon.DOScale(_weaponsFeature.Settings.ArmedWeaponScale, 0.5f).SetDelay(0.2f);
        }
        private void StartTransitionToLevel()
        {
            _gameFeature.OnTransitionToLevel?.Invoke();
        }
        private void RestoreTablePos()
        {
            _weaponHubFeature.Refs.TableTransform.position = _startTablePos;
            _weaponHubFeature.Refs.TableTransform.rotation = _startTableRot;
        }
    }
}