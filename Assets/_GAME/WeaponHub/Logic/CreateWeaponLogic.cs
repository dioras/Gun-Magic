using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace _GAME
{
    public class CreateWeaponLogic : MonoBehaviour
    {
        private WeaponHubFeature _weaponHubFeature;
        private void OnEnable()
        {
            _weaponHubFeature = GameFeature.WeaponHubFeature;

            _weaponHubFeature.OnCreateWeapon += CreateWeapon;
        }

        private void CreateWeapon()
        {
            WeaponRefs weaponPrefab = null;

            foreach (var weapon in _weaponHubFeature.Settings.AvailableWeapons)
            {
                var similarMaterials = _weaponHubFeature.SelectedMaterials.Intersect(weapon.RequiredMaterials);

                if (similarMaterials.Count() == 2)
                {
                    weaponPrefab = weapon;
                    break;
                }
            }
            
            _weaponHubFeature.CraftedWeapon = Instantiate(weaponPrefab, _weaponHubFeature.Refs.WeaponHolder.transform);
            
            this.DelayedCall(0.8f, _weaponHubFeature.OnMoveIntoPortal);

            _weaponHubFeature.CraftedWeapon.transform.DOLocalRotate(Vector3.zero.AddY(600f), 5f, RotateMode.LocalAxisAdd);
        }
    }
}