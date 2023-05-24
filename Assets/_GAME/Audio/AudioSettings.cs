using System;
using UnityEngine;

namespace _GAME.Audio
{
    [Serializable, CreateAssetMenu(fileName = "AudioSettings", menuName = "GAME settings/AudioSettings")]
    public class AudioSettings : ScriptableObject
    {
        public float trackFadeTime;
        public TrackSettings[] tracks;
        public SoundSettings[] sounds;
    }
}