using System;
using UnityEngine;

namespace _GAME.Audio
{
    [Serializable]
    public class SoundSettings
    {
        public EnumSound type;
        public AudioClip clip;
        public bool randomizePitch;
    }
}