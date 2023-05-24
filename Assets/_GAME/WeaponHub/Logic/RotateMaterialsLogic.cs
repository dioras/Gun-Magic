using System.Collections;
using UnityEngine;

namespace _GAME
{
    public class RotateMaterialsLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponHubFeature _weaponHubFeature;

        private void OnEnable()
        {
            _gameFeature = GameFeature.Instance;
            _weaponHubFeature = GameFeature.WeaponHubFeature;

            _gameFeature.OnStartGame += StartMaterialRotation;
            _gameFeature.OnTransitionToLevel += StopMaterialRotation;

            _weaponHubFeature.OnReloadHub += StartMaterialRotation;
        }

        private void StartMaterialRotation()
        {
            StopAllCoroutines();
            StartCoroutine(RotateMaterials());
        }
        private void StopMaterialRotation()
        {
            StopAllCoroutines();
        }
        private IEnumerator RotateMaterials()
        {
            while (true)
            {
                foreach (var weaponMaterial in _weaponHubFeature.SlotMaterials)
                {
                    weaponMaterial.transform.Rotate(Vector3.forward * (_weaponHubFeature.Settings.MaterialRotationSpeed * Time.deltaTime));
                }

                yield return null;
            }
        }
        
    }
}