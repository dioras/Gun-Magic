using _GAME.Common;
using UnityEngine;

namespace _GAME.Enemy
{
    public class DroneRef : MonoBehaviour
    {
        public CollisionCatcher CollisionCatcher;
        public ParticleSystem DamageEffect;
        public GameObject Model;

        public Transform DestructedPartsParent;
        public Rigidbody[] DestructedParts;

        public float ExplosionForce = 3;
        public float DelayToDestroy = 2;

        public EnemyRefs Enemy;

        public bool IsTriggered = false;
        public bool IsCanMove = false;
    }
}
