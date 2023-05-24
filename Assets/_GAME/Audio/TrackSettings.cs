using System;
using UnityEngine;

namespace _GAME.Audio
{
    [Serializable]
    public class TrackSettings
    {
        public EnumTrack type;
        public AudioClip clip;
        public bool looping;
    }
}