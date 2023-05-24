using _GAME.Level;
using UnityEngine;

namespace _GAME
{
    public class EquipWeaponLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private WeaponsFeature _weaponsFeature;
        private WeaponHubFeature _weaponHubFeature;

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
            _weaponsFeature = GameFeature.WeaponsFeature;
            _weaponHubFeature = GameFeature.WeaponHubFeature;

            _levelFeature.OnStartLevel += EquipWeapon;
        }

        private void EquipWeapon()
        {
            var camera = Camera.main.transform;
            var weapon = _weaponHubFeature.CraftedWeapon;

            weapon.transform.SetParent(camera);

            _weaponsFeature.PlayerWeapon = weapon;

            _weaponsFeature.PlayerWeapon.transform.localPosition = weapon.EquippedPosition;
            _weaponsFeature.PlayerWeapon.transform.localRotation = Quaternion.Euler(weapon.EquippedRotation);
        }
    }
}
