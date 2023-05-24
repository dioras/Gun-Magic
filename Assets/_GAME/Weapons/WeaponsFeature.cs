using System;
using UnityEngine;

namespace _GAME
{
    public class WeaponsFeature : MonoBehaviour
    {
        public Action<Vector3> OnShootCurrentWeapon;
        public Action<ArrowRefs, Vector3, Collider> OnArrowHit;
        public Action<IceSphereRefs, Vector3, Collider> OnIceSphereHit;
        public Action<GravitySphereRefs, Vector3, Collider> OnGravitySphereHit;

        public WeaponSettings Settings;

        public WeaponRefs PlayerWeapon;
    }
}