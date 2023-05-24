using UnityEngine;

namespace _GAME.Audio
{
    [RequireComponent(typeof(AudioFeature))]
    public class AudioDebugLogic : MonoBehaviour
    {
        private AudioFeature _feature;

        private void Awake() => _feature = GetComponent<AudioFeature>();
 
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                _feature.PlaySound(EnumSound.SoundTest, 0);
            
            if (Input.GetKeyDown(KeyCode.T))
                _feature.ChangeTrack(EnumTrack.TrackTest, 0);
            
            if (Input.GetKeyDown(KeyCode.Alpha0))
                _feature.ChangeTrackVolume(0);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                _feature.ChangeTrackVolume(50);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                _feature.ChangeTrackVolume(90);
        }
    }
}