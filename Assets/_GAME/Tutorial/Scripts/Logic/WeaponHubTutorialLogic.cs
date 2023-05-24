using _GAME.Level;
using _GAME.LevelUIView;
using System.Linq;
using UnityEngine;

namespace _GAME.Tutorial
{
    public class WeaponHubTutorialLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private WeaponHubFeature _weaponHubFeature;
        private LevelUIFeature _levelUIFeature;

        private Camera _hubCamera;

        private WeaponsHubViewRefs _weaponsHubViewRefs;

        private WeaponMaterialRefs firstMat;
        private WeaponMaterialRefs secondMat;

        private Coroutine _corShowNextTutorial;
        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
            _weaponHubFeature = GameFeature.WeaponHubFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;

            _hubCamera = _weaponHubFeature.Refs.HubCamera;
            _weaponsHubViewRefs = _levelUIFeature.WeaponsHubView;
        }

        private void OnEnable()
        {
            _levelUIFeature.OnLevelLoaded += StartWeaponHubTutorial;
        }

        private void StartWeaponHubTutorial()
        {
            if (!_levelFeature.Level.IsTutorial)
                return;

            if (_levelFeature.StartInBattleMode)
                return;

            var tutorialWeapon = _weaponHubFeature.Settings.AvailableWeapons.First(w => w.Type == _weaponHubFeature.Settings.TutorialWeaponType);
            var tutorialMaterials = tutorialWeapon.RequiredMaterials;

            _weaponHubFeature.Refs.Forge1.TapCatcher.IsActive = false;
            _weaponHubFeature.Refs.Forge2.TapCatcher.IsActive = false;

            foreach (var materialSlot in _weaponHubFeature.Refs.Slots)
            {
                if (materialSlot.WeaponMaterial.type == tutorialMaterials[0])
                {
                    firstMat = materialSlot.WeaponMaterial;
                }
                else if (materialSlot.WeaponMaterial.type == tutorialMaterials[1])
                {
                    secondMat = materialSlot.WeaponMaterial;
                    materialSlot.TapCatcher.IsActive = false;
                }
                else
                    materialSlot.TapCatcher.IsActive = false;
            }

            _weaponsHubViewRefs.View.Show();
            firstMat.parentSlot.TapCatcher.OnDown += ShowSelectSecondMatTutorial;

            _corShowNextTutorial = this.DelayedCall(0.4f, ShowSelectFirstMatTutorial);
        }

        private void ShowSelectFirstMatTutorial()
        {
            if (_corShowNextTutorial != null)
                StopCoroutine(_corShowNextTutorial);

            var firstTutorialHandPos = _hubCamera.WorldToScreenPoint(firstMat.transform.position);

            _weaponsHubViewRefs.TutorialHand.position = firstTutorialHandPos;
            _weaponsHubViewRefs.TutorialHand.gameObject.SetActive(true);
        }

        private void ShowSelectSecondMatTutorial(Transform sender)
        {
            firstMat.parentSlot.TapCatcher.OnDown -= ShowSelectSecondMatTutorial;

            _weaponsHubViewRefs.TutorialHand.gameObject.SetActive(false);
            firstMat.parentSlot.TapCatcher.IsActive = false;

            secondMat.parentSlot.TapCatcher.IsActive = true;
            secondMat.parentSlot.TapCatcher.OnDown += FinishHubTutorial;

            _corShowNextTutorial = this.DelayedCall(0.4f, () =>
             {
                 var secondTutorialHandPos = _hubCamera.WorldToScreenPoint(secondMat.transform.position);

                 _weaponsHubViewRefs.TutorialHand.position = secondTutorialHandPos;
                 _weaponsHubViewRefs.TutorialHand.gameObject.SetActive(true);
             });
        }

        private void FinishHubTutorial(Transform sender)
        {
            if (_corShowNextTutorial != null)
                StopCoroutine(_corShowNextTutorial);

            secondMat.parentSlot.TapCatcher.OnDown -= FinishHubTutorial;

            _weaponsHubViewRefs.TutorialHand.gameObject.SetActive(false);

            foreach (var materialSlot in _weaponHubFeature.Refs.Slots)
                materialSlot.TapCatcher.IsActive = true;
        }
    }
}