using UnityEngine;

namespace _GAME.Enemy
{
    [CreateAssetMenu(fileName = "EnemyEffectsPreset", menuName = "GAME settings/EnemyEffectsPreset")]
    public class EnemyEffectsPreset : ScriptableObject
    {
        public ParticleSystem ElectroEffect;
        public ParticleSystem BoomEnemyEffect;
    }
}
