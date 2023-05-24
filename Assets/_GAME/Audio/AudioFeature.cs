using _GAME.SaveAndLoadData;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Audio
{
    public class AudioFeature : MonoBehaviour
    {
        public System.Action OnSaveMusicSettings;
        
        
        public AudioSettings settings;

        public AudioSource[] soundsAudioSources;
        public AudioSource[] tracksAudioSources;

        public IconSwitchRefs MusicIcon;

        [ReadOnly] public float trackVolumeInPercent;
        [ReadOnly] public float soundsVolumeInPercent;

        public bool IsSoundOn;

#pragma warning disable 0103
#pragma warning disable 0414
        private EnumTrack _currentTrack;
#pragma warning restore 0103
#pragma warning restore 0414

        private SaveAndLoadDataFeature _saveAndLoadDataFeature;

        private void Awake()
        {
            _saveAndLoadDataFeature = FindObjectOfType<SaveAndLoadDataFeature>();
        }

        private void OnEnable()
        {
            _saveAndLoadDataFeature.OnPrepareDataForSaving += SaveData;
            
        }

        private void OnDisable()
        {
            _saveAndLoadDataFeature.OnPrepareDataForSaving -= SaveData;
        }

        private void SaveData()
        {
            _saveAndLoadDataFeature.Data.soundsVolumeInPercent = soundsVolumeInPercent;
            _saveAndLoadDataFeature.Data.trackVolumeInPercent = trackVolumeInPercent;
        }

        public void Init()
        {
            if (!_saveAndLoadDataFeature.DataLoaded)
            {
                ChangeSoundsVolume(100);
                ChangeTrackVolume(100);
            }
            else
            {
                ChangeSoundsVolume(_saveAndLoadDataFeature.Data.soundsVolumeInPercent);
                ChangeTrackVolume(_saveAndLoadDataFeature.Data.trackVolumeInPercent);
            }
        }

        public void ChangeSoundsVolume(float volumeLevelInPercent)
        {
            soundsVolumeInPercent = volumeLevelInPercent;
            
            foreach (var audioSource in soundsAudioSources)
                audioSource.volume = volumeLevelInPercent / 100f;
        }
        
        public void ChangeTrackVolume(float volumeLevelInPercent)
        {
            trackVolumeInPercent = volumeLevelInPercent;
            
            foreach (var audioSource in tracksAudioSources)
                audioSource.volume = volumeLevelInPercent / 100f;
        }

        public void ChangeTrack(EnumTrack track, float delay)
        {
            var busyAudioSource = tracksAudioSources.ToList().Find(s => s.isPlaying);
            if (busyAudioSource != null)
                busyAudioSource.DOFade(0, settings.trackFadeTime)
                    .OnComplete(() => DisableTrackAudioSource(busyAudioSource));
            
            var freeAudioSource = tracksAudioSources.ToList().Find(s => !s.isPlaying);
            if (freeAudioSource == null)
            {
                Debug.LogWarning("No more free audio sources for tracks!");
                return;
            }
            
            var trackSettings = settings.tracks.ToList().Find(s => s.type == track);
            if (trackSettings == null)
            {
                Debug.LogWarning($"No settings for track of type {track:F}");
                return;
            }
            
            freeAudioSource.clip = trackSettings.clip;
            freeAudioSource.PlayDelayed(delay);
            freeAudioSource.loop = trackSettings.looping;
            if (!trackSettings.looping)
                StartCoroutine(WaitAndChangeCurrentTrackToNone(trackSettings.clip.length));
        }
        
        private IEnumerator WaitAndChangeCurrentTrackToNone(float delay)
        {
            yield return new WaitForSeconds(delay);
            _currentTrack = EnumTrack.None;
        }

        private void DisableTrackAudioSource(AudioSource audioSource)
        {
            audioSource.clip = null;
            audioSource.Stop();
            audioSource.volume = trackVolumeInPercent / 100f;
        }
        
        public void PlaySound(EnumSound sound, float delay = 0, float volume = 1, AudioSource audioSource = null)
        {
            if (!IsSoundOn) return;
            
            AudioSource freeAudioSource = audioSource;

            if (freeAudioSource == null)
            {
                freeAudioSource = soundsAudioSources.ToList().Find(s => !s.isPlaying);
                if (freeAudioSource == null)
                {
                    Debug.LogWarning("No more free audio sources for sounds!");
                    return;
                }
            }

            var soundSettings = settings.sounds.ToList().Find(s => s.type == sound);
            if (soundSettings == null)
            {
                Debug.LogWarning($"No settings for sound of type {sound:F}");
                return;
            }

            freeAudioSource.clip = soundSettings.clip;
            if (soundSettings.randomizePitch)
                freeAudioSource.pitch = Random.Range(0.9f, 1.1f);
            else
                freeAudioSource.pitch = 1;
            freeAudioSource.volume = volume;
            freeAudioSource.PlayDelayed(delay);
        }
    }
}