using UnityEngine;

namespace _GAME.Enemy
{
    [System.Serializable, CreateAssetMenu(fileName = "EnemySettings", menuName = "GAME settings/EnemySettings")]
    public class EnemySettings : ScriptableObject
    {
        //public ParticleSystem ElectoEffect;
        //public ParticleSystem BoomEnemyEffect;
        
        public float MoveSpeed;

        public float StopDistance;
        public float StopDistanceForDrone;

        public string AnimationMoveKey = "Walk";
        public string AnimationAttackKey = "Attack";
        public string AnimationFallKey = "Fall";
        public string AnimationThrowKey = "Throw";

        public LayerMask DamagableLayer;
        public LayerMask DestructableLayer;
        public LayerMask GroundLayer;
        public LayerMask EnemyLayer;
        public LayerMask WallLayer;
        public LayerMask WaterLayer;

        public int DamagableLayerNumber = 10;

        //public EnemyRefs[] EnemyPrefabs;

        public EnemyPrefabsPreset EnemyPrefabsPreset;
        public EnemyEffectsPreset EnemyEffectsPreset;

        public Color ColorOnDie;

    }
}
