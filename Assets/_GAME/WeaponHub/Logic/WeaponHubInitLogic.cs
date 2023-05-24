using System.Linq;
using UnityEngine;
using _GAME.Common;
using _GAME.LevelUIView;
using _GAME.Level;

namespace _GAME
{
    public class WeaponHubInitLogic : MonoBehaviour
    {
        private WeaponHubFeature _weaponHubFeature;
        private LevelFeature _levelFeature;

        private bool _isInitialized = false;

        private void OnEnable()
        {
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _levelFeature = GameFeature.LevelFeature;

            _levelFeature.OnLevelLoaded += InitHub;
        }

        private void InitHub(LevelRefs level)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                InstantiateWeaponMaterials();
            }
            else
            {
                ReloadHub();
            }
        }

        private void InstantiateWeaponMaterials()
        {
            if (_levelFeature.Level.IsTutorial)
            {
                InstantiateTutorialMaterials();
                return;
            }

            var materialPool = _weaponHubFeature.Settings.AvailableWeaponMaterials.ToList();

            for (int i = 0; i < _weaponHubFeature.Refs.Slots.Length; i++)
            {
                var slot = _weaponHubFeature.Refs.Slots[i];
                var materialPrefab = materialPool.RandomElement();

                materialPool.Remove(materialPrefab);
                InstantiateMaterial(slot, materialPrefab);
            }
        }

        private void InstantiateTutorialMaterials()
        {
            var tutorialWeapon = _weaponHubFeature.Settings.AvailableWeapons.First(w => w.Type == _weaponHubFeature.Settings.TutorialWeaponType);
            var tutorialMaterials = tutorialWeapon.RequiredMaterials;

            var materialPool = _weaponHubFeature.Settings.AvailableWeaponMaterials.Where(mat => !tutorialMaterials.Contains(mat.type)).ToList();

            //Slots from right to left
            //Slot 1
            var slot = _weaponHubFeature.Refs.Slots[0];
            var materialPrefab = materialPool.RandomElement();
            
            materialPool.Remove(materialPrefab);
            InstantiateMaterial(slot, materialPrefab);

            //Slot 2
            slot = _weaponHubFeature.Refs.Slots[1];
            materialPrefab = _weaponHubFeature.Settings.AvailableWeaponMaterials.First(mat => mat.type == tutorialMaterials[1]);

            InstantiateMaterial(slot, materialPrefab);

            //Slot 3
            slot = _weaponHubFeature.Refs.Slots[2];
            materialPrefab = materialPool.RandomElement();

            materialPool.Remove(materialPrefab);
            InstantiateMaterial(slot, materialPrefab);

            //Slot 4
            slot = _weaponHubFeature.Refs.Slots[3];
            materialPrefab = _weaponHubFeature.Settings.AvailableWeaponMaterials.First(mat => mat.type == tutorialMaterials[0]);

            InstantiateMaterial(slot, materialPrefab);

            //Slot 5
            slot = _weaponHubFeature.Refs.Slots[4];
            materialPrefab = materialPool.RandomElement();

            materialPool.Remove(materialPrefab);
            InstantiateMaterial(slot, materialPrefab);
        }
        private void InstantiateMaterial(MaterialSlotRefs slot, WeaponMaterialRefs materialPrefab)
        {
            slot.WeaponMaterial = Instantiate(materialPrefab, slot.transform);

            slot.WeaponMaterial.parentSlot = slot;

            slot.WeaponMaterial.transform.localPosition = Vector3.zero.AddZ(_weaponHubFeature.Settings.MaterialSlotOffset);
            slot.WeaponMaterial.transform.localRotation = Quaternion.Euler(Vector3.zero);

            _weaponHubFeature.SlotMaterials.Add(slot.WeaponMaterial);
            slot.isEmpty = false;

            slot.WeaponMaterial.isSelectable = true;

            slot.WeaponMaterial.originalMaterial = slot.WeaponMaterial.renderers[0].sharedMaterial;
        }
        private void ReloadHub()
        {
            Destroy(_weaponHubFeature.CraftedWeapon.gameObject);

            foreach (var slot in _weaponHubFeature.Refs.Slots)
                Destroy(slot.WeaponMaterial.gameObject);

            _weaponHubFeature.SlotMaterials.Clear();

            InstantiateWeaponMaterials();

            _weaponHubFeature.OnReloadHub?.Invoke();

            _weaponHubFeature.Refs.HubCamera.transform.localPosition = _weaponHubFeature.Settings.CameraPosition;
            _weaponHubFeature.Refs.HubCamera.transform.localRotation = Quaternion.Euler(_weaponHubFeature.Settings.CameraRotation);
            _weaponHubFeature.Refs.HubCamera.gameObject.SetActive(true);
        }
    }
}