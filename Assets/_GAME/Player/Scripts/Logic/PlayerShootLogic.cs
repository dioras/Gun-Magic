using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using _GAME.Level;
using _GAME.LevelUIView;

namespace _GAME.Player
{
    public class PlayerShootLogic : MonoBehaviour
    {
        private PlayerFeature _playerFeature;
        private LevelFeature _levelFeature;
        private LevelUIFeature _levelUI;
        private WeaponsFeature _weaponFeature;
        
        private Camera cam;

        private PlayerSettings _settings;

        private void Awake()
        {
            _playerFeature = GameFeature.PlayerFeature;
            _levelFeature = GameFeature.LevelFeature;
            _levelUI = GameFeature.LevelUIFeature;
            _weaponFeature = GameFeature.WeaponsFeature;

            _settings = _playerFeature.PlayerSettings;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += InitPlayer;
            _levelFeature.OnStartLevel += PreparePlayer;
            _levelUI.OnClickNextLevel += () => _playerFeature.CanShoot = false;
            _levelFeature.OnLevelFailed += () => _playerFeature.CanShoot = false;

            _playerFeature.OnTutorialShoot += TutorialShoot;
        }
        private void InitPlayer(Level.LevelRefs level)
        {
            _playerFeature.Player = _levelFeature.LevelBase.Player;
            _playerFeature.CanShoot = false;
            _playerFeature.IsMoving = false;
            cam = Camera.main;
        }
        private void PreparePlayer()
        {
            this.DelayedCall(0.25f, () => _playerFeature.CanShoot = true);
        }

        private void Update()
        {
            if (_levelFeature.Level == null || _playerFeature.Player == null) return;

            if (!_playerFeature.CanShoot || _playerFeature.IsMoving == true) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_levelFeature.Level.IsTutorial && _playerFeature.CurrentPlatform == 1)
                    return;

                var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
                var ray = cam.ScreenPointToRay(mousePos);
                Shoot(ray.GetPoint(20f));
            }
        }

        private void TutorialShoot(Vector3 tutorialBtnPos)
        {
            var ray = cam.ScreenPointToRay(tutorialBtnPos);
            Shoot(ray.GetPoint(20f));
        }

        private void Shoot(Vector3 shootPos)
        {
            _playerFeature.CanShoot = false;

            _weaponFeature.PlayerWeapon.Animator.SetTrigger("Shoot");

            _weaponFeature.OnShootCurrentWeapon?.Invoke(shootPos);

            this.DelayedCall(_playerFeature.DelayBeforeShoot, () =>
            {
                float delayBeforeNextShot = 0.2f; //TODO: вынести в настройки

                this.DelayedCall(delayBeforeNextShot, () =>
                {
                    _playerFeature.CanShoot = true;
                });
            });
        }

        private void SubscribeBullet(BulletRefs bullet, Collider hitCollider)
        {
            bullet.CollisionCatcher.OnTriggerEnterEvent += (col) => BulletCollisionProcess(col, bullet, hitCollider);
        }

        private void BulletCollisionProcess(Collider col, BulletRefs bullet, Collider hitCollider)
        {
            if ((1 << col.gameObject.layer) != 0) return;

            if (!bullet.IsTriggered)
            {
                bullet.IsTriggered = true;

                _playerFeature.ActiveBullet = null;

                Destroy(bullet.gameObject);
            }
        }

        private List<RaycastResult> _results = new List<RaycastResult>();
        
        private bool IsPointerOverUIObject()
        {
            _results.Clear();
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            EventSystem.current.RaycastAll(eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }
    }
}
