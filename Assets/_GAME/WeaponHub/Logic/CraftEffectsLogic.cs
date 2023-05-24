using _GAME.Audio;
using _GAME.VibroService;
using DG.Tweening;
using System;
using UnityEngine;

namespace _GAME
{
    public class CraftEffectsLogic : MonoBehaviour
    {
        private WeaponHubFeature _weaponHubFeature;
        private AudioFeature _audioFeature;
        private VibroFeature _vibroFeature;

        private void OnEnable()
        {
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _audioFeature = GameFeature.AudioFeature;
            _vibroFeature = GameFeature.VibroFeature;

            _weaponHubFeature.OnShowCraftEffects += ShowCraftEffects;
        }

        private void ShowCraftEffects()
        {
            var leftMaterial = _weaponHubFeature.Refs.Forge1.WeaponMaterial;
            var rightMaterial = _weaponHubFeature.Refs.Forge2.WeaponMaterial;

            var mergePos = _weaponHubFeature.Refs.MergeEpicenter.position;

            leftMaterial.transform.DOScale(1.5f, 0.3f);
            rightMaterial.transform.DOScale(1.5f, 0.3f);
            leftMaterial.transform.DOShakePosition(0.3f, strength: 1.2f, vibrato: 50);
            rightMaterial.transform.DOShakePosition(0.3f, strength: 1.2f, vibrato: 50).OnComplete(() =>
            {
                leftMaterial.transform.DOMove(mergePos, 0.2f);
                rightMaterial.transform.DOMove(mergePos, 0.2f).OnComplete(() =>
                {
                    leftMaterial.gameObject.SetActive(false);
                    rightMaterial.gameObject.SetActive(false);

                    _weaponHubFeature.Refs.MergeEffect.Play();
                    _audioFeature.PlaySound(EnumSound.MergeMaterials);

                    ActivateWeaponForge();
                });
            });

            _vibroFeature.OnVibrateHard?.Invoke();
        }

        private void ActivateWeaponForge()
        {
            foreach (var pillarLightning in _weaponHubFeature.Refs.PillarLightnings)
                pillarLightning.SetActive(false);

            foreach (var pillarLightning in _weaponHubFeature.Refs.TableLightnings)
                pillarLightning.SetActive(true);

            this.DelayedCall(0.68f, CreateWeapon);
        }

        private void CreateWeapon()
        {
            foreach (var pillarLightning in _weaponHubFeature.Refs.TableLightnings)
                pillarLightning.SetActive(false);

            _weaponHubFeature.Refs.CraftEffect.Play();

            _audioFeature.PlaySound(EnumSound.CreateWeapon);

            _weaponHubFeature.OnCreateWeapon?.Invoke();
        }
    }
}