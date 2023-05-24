using Doozy.Engine.Soundy;
using UnityEngine;

namespace _GAME.Audio
{
    public class AudioSwitchViewLogic : MonoBehaviour
    {
        
        private AudioFeature _audioFeature;
        private SaveAndLoadData.SaveAndLoadDataFeature _saveAndLoadDataFeature;

        private IconSwitchRefs _icon;

        private void Awake()
        {
            _audioFeature = GameFeature.AudioFeature;
            _saveAndLoadDataFeature = GameFeature.SaveAndLoadDataFeature;
            _saveAndLoadDataFeature.OnDataLoaded += UpdateAudioState;

            _icon = _audioFeature.MusicIcon;
        }

        private void UpdateAudioState()
        {
            _audioFeature.IsSoundOn = _saveAndLoadDataFeature.Data.SoundsOn;

            _icon.OnImage.enabled = _audioFeature.IsSoundOn;
            _icon.OffImage.enabled = !_audioFeature.IsSoundOn;

            if (_icon.BG)
            {
                _icon.BG.color = _icon.OnImage.enabled ? _icon.ColorOn : _icon.ColorOff;
            }

            UpdateDoozySoundManagers(_audioFeature.IsSoundOn);
        }

        private void OnEnable()
        {
            _icon.ButtonSwitch.onClick.AddListener(SwitchState);
        }

        private void SwitchState()
        {
            _icon.State = !_icon.State;

            _icon.OffImage.enabled = !_icon.State;
            _icon.OnImage.enabled = _icon.State;

            if (_icon.BG)
            {
                _icon.BG.color = _icon.OnImage.enabled ? _icon.ColorOn : _icon.ColorOff;
            }

            _audioFeature.IsSoundOn = _icon.State;

            UpdateDoozySoundManagers(_audioFeature.IsSoundOn);

            _saveAndLoadDataFeature.Data.SoundsOn = _audioFeature.IsSoundOn;
            
            _audioFeature.OnSaveMusicSettings?.Invoke();
        }

        private void UpdateDoozySoundManagers(bool value)
        {
            if (!value)
            {
                SoundyManager.MuteAllControllers();
                SoundyController.MuteAll();
                SoundyManager.MuteAllSounds();
            }
            else
            {
                SoundyManager.UnmuteAllControllers();
                SoundyController.UnmuteAll();
                SoundyManager.UnmuteAllSounds();
            }
        }





        //         [SerializeField] private AudioSwitchViewRefs viewRefs;
        //         
        //         private AudioFeature _audio;
        //         private LevelsFeature _levels;
        //
        //         private void Awake()
        //         {
        //             _audio = FindObjectOfType<AudioFeature>();
        //             _levels = FindObjectOfType<LevelsFeature>();
        //         }
        //          
        //         private void OnEnable()
        //         {
        //             viewRefs.btn.onClick.AddListener(SwitchAudio);
        //             _levels.OnStatesChanged += TryShowOrHideView;
        //         }
        //
        //         private void OnDisable()
        //         {
        //             viewRefs.btn.onClick.RemoveListener(SwitchAudio);
        //             _levels.OnStatesChanged -= TryShowOrHideView;
        //         }
        //
        //         private void TryShowOrHideView()
        //         {
        //             if (_levels.justAddedStates.Contains(EnumLevelState.Lobby))
        //             {
        //                 viewRefs.view.Show();
        //                 RefreshView();
        //             }
        //             else if (_levels.justRemovedStates.Contains(EnumLevelState.Lobby))
        //                 viewRefs.view.Hide();
        //         }
        //
        //         private void SwitchAudio()
        //         {
        //             var newVolume = _audio.soundsVolumeInPercent > 0 ? 0 : 100f;
        //             _audio.ChangeSoundsVolume(newVolume);
        //             _audio.ChangeTrackVolume(newVolume);
        //             RefreshView();
        //         }
        //
        //         private void RefreshView()
        //         {
        //             viewRefs.btnImage.sprite = _audio.soundsVolumeInPercent > 0
        //                 ? viewRefs.spriteOn
        //                 : viewRefs.spriteOff;
        //         }
    }
}