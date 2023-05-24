using UnityEngine;

namespace _GAME.Player
{
    public class MoveBulletLogic : MonoBehaviour
    {
        private PlayerFeature _playerFeature;

        private void Awake()
        {
            _playerFeature = GameFeature.PlayerFeature;
        }

        private void Update()
        {
            if (_playerFeature.ActiveBullet && _playerFeature.ActiveBullet.IsCanMove)
            {
                var bullet = _playerFeature.ActiveBullet;
                var dir = bullet.EndPoint - bullet.transform.position;
                bullet.RootTransform.Translate(dir * bullet.Speed * Time.deltaTime, Space.World);
            }
        }
    }
}
