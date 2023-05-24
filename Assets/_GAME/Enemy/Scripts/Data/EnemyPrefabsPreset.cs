using UnityEngine;

namespace _GAME.Enemy
{
    [CreateAssetMenu(fileName = "EnemyPrefabsPreset", menuName = "GAME settings/EnemyPrefabsPreset")]
    public class EnemyPrefabsPreset : ScriptableObject
    {
        public EnemyRefs[] EnemyPrefabs;
        public Avatar[] EnemyFloatingAvatars;
    }
}
