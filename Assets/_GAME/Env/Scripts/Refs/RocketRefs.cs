using _GAME.Common;
using DG.Tweening;
using UnityEngine;
using _GAME.Enemy;

namespace _GAME.Env
{
    public class RocketRefs : MonoBehaviour
    {
        public DOTweenPath FlyPath;
        public float SpeedFly = 10;
        public float SpeedRotate = 10;
        public Transform RocketModel;
        public Transform EffectBoomPoint;
        public CollisionCatcher CollisionCatcher;
        public Collider MagnetCollider;
        public AudioSource AudioSource;

        public Vector3[] FlyPoints;

        [Header("Destruct parts")]
        public Transform PartsRoot;
        public Rigidbody[] Parts;
        public float DelayToHideParts = 2;
        
        //[Header("Scene References")]
        [HideInInspector] public EnemyRefs Target;
    }
}
