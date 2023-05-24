using System;
using UnityEngine;

namespace _GAME
{
    [Serializable, CreateAssetMenu(fileName = "WeaponSettings", menuName = "GAME settings/WeaponSettings")]
    public class WeaponSettings : ScriptableObject
    {
        public Vector3 ArmedWeaponPosition;
        public Vector3 ArmedWeaponRotation;
        public float ArmedWeaponScale;

        public Vector3 LevelWeaponOffset;

        public LayerMask BulletLayer;

        public BalloonCrossbowSettings BalloonCrossbowSettings;
        public FrostKeeperSettings FrostKeeperSettings;
        public GravityGunSettings GravityGunSettings;
    }

    [Serializable]
    public class BalloonCrossbowSettings
    {
        public ArrowRefs ArrowPrefab;
        public FlyingBalloonRefs FlyingBalloonPrefab;
        public float ArrowSpeed;
        public float LaunchedArrowScale;
    }

    [Serializable]
    public class FrostKeeperSettings
    {
        public IceSphereRefs IceSpherePrefab;
        public GameObject SphereExplosionPrefab;
        public float IceSphereSpeed;
        public float IceExplosionStrenght;
    }

    [Serializable]
    public class GravityGunSettings
    {
        public GravitySphereRefs GravitySpherePrefab;
        public float GravitySphereSpeed;

        public float ExplosionRadius;
        public float ExploadingSphereScale;
        public ParticleSystem ExplosionParticles;

        public float LevitationMinY;
        public float LevitationMaxY;
        public float LevitationDuration;
        public float IntakeDuration;

        public int FloatingAnimationsCount;

        public float SphereImploadDelay;
    }
}