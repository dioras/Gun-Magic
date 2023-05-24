using UnityEngine;

namespace _GAME
{
    public class WeaponRefs : MonoBehaviour
    {
        public EnumWeaponType Type;
        public EnumWeaponMaterialType[] RequiredMaterials;

        public Transform bulletSpawnAnchor;
        public Animator Animator;

        public Vector3 EquippedPosition;
        public Vector3 EquippedRotation;
    }

    public enum EnumWeaponType
    {
        None = 0,
        FrostKeeper = 1,
        BaloonCrossbow = 2,
        GravityGun = 3,
    }
}
