using UnityEngine;

namespace _GAME.Player
{
    public class BulletRefs : MonoBehaviour
    {
        public Transform RootTransform;
        public float Speed;
        public bool IsCanMove = false;
        public Vector3 EndPoint;
        public Vector3 HitNormal;
        public bool IsTriggered = false;

        public Common.CollisionCatcher CollisionCatcher;
    }
}
