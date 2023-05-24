using System;
using UnityEngine;

namespace _GAME
{
    [Serializable, CreateAssetMenu(fileName = "LevelSettings", menuName = "GAME settings/LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        public LayerMask GroundLayer;
        public LayerMask WallLayer;
        public LayerMask EnemyLayer;
        public LayerMask BulletLayer;
    }
}