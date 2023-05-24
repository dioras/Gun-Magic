using _GAME.Common;
using UnityEngine;

namespace _GAME.Env
{
    public class ExplosionRefs : MonoBehaviour
    {
        public CollisionCatcher CollisionCatcher;
        public SphereCollider DamagableTrigger;
        public ParticleSystem ExplosionEffect;
        public Rigidbody Rigidbody;
        public GameObject Model;

        public float DamagableTriggerRadius;

        public bool IsTriggered = false;
    }
}
