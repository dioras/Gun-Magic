using _GAME.Audio;
using System;
using System.Linq;
using UnityEngine;

namespace _GAME
{
    public class MaterialSelectionLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponHubFeature _weaponHubFeature;
        private AudioFeature _audioFeature;

        private bool isInitialized = false;

        private bool isCraftMode = true;

        private void OnEnable()
        {
            _gameFeature = GameFeature.Instance;
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _audioFeature = GameFeature.AudioFeature;

            _gameFeature.OnStartGame += InitMaterialSelection;
            _weaponHubFeature.OnReloadHub += ResetSelectionLogic;
        }

        private void InitMaterialSelection()
        {
            if (isInitialized)
                return;

            foreach (var slot in _weaponHubFeature.Refs.Slots)
            {
                slot.TapCatcher.OnDown += SelectMaterial;
            }

            _weaponHubFeature.Refs.Forge1.TapCatcher.OnDown += DeselectMaterial;
            _weaponHubFeature.Refs.Forge2.TapCatcher.OnDown += DeselectMaterial;

            _weaponHubFeature.Refs.Forge1.isEmpty = true;
            _weaponHubFeature.Refs.Forge2.isEmpty = true;

            isInitialized = true;
        }

        private void SelectMaterial(Transform sender)
        {
            if (!isCraftMode)
                return;

            var slot = sender.GetComponent<MaterialSlotRefs>();

            if (slot.isEmpty)
                return;

            var selectedMaterial = slot.WeaponMaterial;

            if (!selectedMaterial.isSelectable)
                return;

            _audioFeature.PlaySound(EnumSound.ButtonClicked);

            slot.isEmpty = true;

            var hubRefs = _weaponHubFeature.Refs;

            ForgeRefs emptyForge;
            if (hubRefs.Forge1.isEmpty)
            {
                emptyForge = hubRefs.Forge1;

                foreach (var pillarLightning in _weaponHubFeature.Refs.PillarLightnings)
                    pillarLightning.SetActive(true);

                foreach (var remainingMaterial in _weaponHubFeature.SlotMaterials)
                {
                    if (remainingMaterial == selectedMaterial)
                        continue;

                    if (!selectedMaterial.compatibleMaterials.Contains(remainingMaterial.type))
                        DeactivateMaterial(remainingMaterial);
                }
            }
            else
                emptyForge = hubRefs.Forge2;

            selectedMaterial.transform.position = emptyForge.transform.position.AddY(_weaponHubFeature.Settings.MaterialForgeOffset);

            emptyForge.WeaponMaterial = selectedMaterial;
            emptyForge.isEmpty = false;

            if (!hubRefs.Forge2.isEmpty)
            {
                _weaponHubFeature.SelectedMaterials.Clear();
                _weaponHubFeature.SelectedMaterials.Add(_weaponHubFeature.Refs.Forge1.WeaponMaterial.type);
                _weaponHubFeature.SelectedMaterials.Add(_weaponHubFeature.Refs.Forge2.WeaponMaterial.type);

                isCraftMode = false;
                _weaponHubFeature.OnShowCraftEffects?.Invoke();
            }
        }

        private void DeselectMaterial(Transform sender)
        {
            if (!isCraftMode)
                return;

            var forge = sender.GetComponent<ForgeRefs>();

            if (forge.isEmpty)
                return;

            _audioFeature.PlaySound(EnumSound.ButtonClicked);

            if (forge == _weaponHubFeature.Refs.Forge1)
            {
                foreach (var pillarLightning in _weaponHubFeature.Refs.PillarLightnings)
                    pillarLightning.SetActive(false);
            }

            forge.WeaponMaterial.transform.localPosition = Vector3.zero.AddZ(_weaponHubFeature.Settings.MaterialSlotOffset);
            forge.WeaponMaterial.parentSlot.isEmpty = false;

            forge.WeaponMaterial = null;
            forge.isEmpty = true;

            foreach (var slotMaterial in _weaponHubFeature.SlotMaterials)
                ReactivateMaterial(slotMaterial);
        }
        private void DeactivateMaterial(WeaponMaterialRefs weaponMaterial)
        {
            weaponMaterial.isSelectable = false;

            foreach (var renderer in weaponMaterial.renderers)
                renderer.material = _weaponHubFeature.Settings.DisabledMaterial;
        }
        private static void ReactivateMaterial(WeaponMaterialRefs weaponMaterial)
        {
            weaponMaterial.isSelectable = true;

            foreach (var renderer in weaponMaterial.renderers)
                renderer.material = weaponMaterial.originalMaterial;
        }
        private void ResetSelectionLogic()
        {
            _weaponHubFeature.SelectedMaterials.Clear();

            _weaponHubFeature.Refs.Forge1.isEmpty = true;
            _weaponHubFeature.Refs.Forge2.isEmpty = true;

            _weaponHubFeature.Refs.Forge1.TapCatcher.IsActive = true;
            _weaponHubFeature.Refs.Forge2.TapCatcher.IsActive = true;

            isCraftMode = true;
        }
    }
}