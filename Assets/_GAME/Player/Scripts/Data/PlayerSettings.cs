using UnityEngine;

namespace _GAME.Player
{
    [System.Serializable, CreateAssetMenu(fileName = "PlayerSettings", menuName = "GAME settings/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public LayerMask ShootableLayer;
        public LayerMask GroundLayer;

        public LayerMask PlayerLayer;
        public LayerMask BulletLayer;
        public LayerMask WaypointLayer;
        public LayerMask WaypointFinish;

        public BulletRefs OrangeBulletPrefab;
        public BulletRefs BlueBulletPrefab;

        public float BulletSpeed = 5;
        public float PlayerMoveSpeed = 10;
        public float ShootRaycastLength = 25;

        public string AnimationShootKey = "Shoot";

        public Vector3 OnHelicopterPosition;
        public Vector3 OnHelicopterRotationAngles;
        public float JumpToHelicopterDuration;
    }
}