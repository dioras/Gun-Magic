using _GAME.SaveAndLoadData;
using MoreMountains.NiceVibrations;
using UnityEngine;


namespace _GAME.VibroService
{
    public class VibroLogic : MonoBehaviour
    {
        private VibroFeature _vibroFeature;
        private SaveAndLoadDataFeature _saveAndLoadDataFeature;

        private void Awake()
        {
            _vibroFeature = GameFeature.VibroFeature;
            _saveAndLoadDataFeature = GameFeature.SaveAndLoadDataFeature;
            _saveAndLoadDataFeature.OnDataLoaded += UpdateVibroState;
        }

        private void UpdateVibroState()
        {
            _vibroFeature.IsVibroOn = _saveAndLoadDataFeature.Data.VibrateOn;

            if (_vibroFeature.Icon)
            {
                _vibroFeature.Icon.OnImage.enabled = _vibroFeature.IsVibroOn;
                _vibroFeature.Icon.OffImage.enabled = !_vibroFeature.IsVibroOn;

                if (_vibroFeature.Icon.BG)
                {
                    _vibroFeature.Icon.BG.color = _vibroFeature.Icon.OnImage.enabled ?
                                                 _vibroFeature.Icon.ColorOn : _vibroFeature.Icon.ColorOff;
                }
            }
        }

        private void OnEnable()
        {
            _vibroFeature.OnVibrateLevelComplete += VibroSucces;
            _vibroFeature.OnVibrateLevelFailed += VibroFailed;
            _vibroFeature.OnVibrateLight += VibroLight;
            _vibroFeature.OnVibrateMedium += VibroMedium;
            _vibroFeature.OnVibrateHard += VibroHard;

            if (_vibroFeature.Icon) _vibroFeature.Icon.ButtonSwitch.onClick.AddListener(SwitchState);
        }

        private void OnDisable()
        {
            _vibroFeature.OnVibrateLevelComplete -= VibroSucces;
            _vibroFeature.OnVibrateLevelFailed -= VibroFailed;
            _vibroFeature.OnVibrateLight -= VibroLight;
            _vibroFeature.OnVibrateMedium -= VibroMedium;
            _vibroFeature.OnVibrateHard -= VibroHard;

            if (_vibroFeature.Icon) _vibroFeature.Icon.ButtonSwitch.onClick.RemoveListener(SwitchState);
        }

        private void SwitchState()
        {
            _vibroFeature.Icon.State = !_vibroFeature.Icon.State;

            _vibroFeature.Icon.OffImage.enabled = !_vibroFeature.Icon.State;
            _vibroFeature.Icon.OnImage.enabled = _vibroFeature.Icon.State;

            if (_vibroFeature.Icon.BG)
            {
                _vibroFeature.Icon.BG.color = _vibroFeature.Icon.OnImage.enabled ?
                                             _vibroFeature.Icon.ColorOn : _vibroFeature.Icon.ColorOff;
            }

            _vibroFeature.IsVibroOn = _vibroFeature.Icon.State;

            _saveAndLoadDataFeature.Data.VibrateOn = _vibroFeature.IsVibroOn;

            _vibroFeature.OnSave?.Invoke();
        }

        private void VibroSucces()
        {
            if (_vibroFeature.IsVibroOn) MMVibrationManager.Haptic(HapticTypes.Success);
        }

        private void VibroFailed()
        {
            if (_vibroFeature.IsVibroOn) MMVibrationManager.Haptic(HapticTypes.Failure);
        }

        private void VibroLight()
        {
            if (_vibroFeature.IsVibroOn) MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

        private void VibroMedium()
        {
            if (_vibroFeature.IsVibroOn) MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        private void VibroHard()
        {
            if (_vibroFeature.IsVibroOn) MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }
    }
}
