using UnityEngine;

namespace _GAME
{
    public class WeaponMaterialRefs : MonoBehaviour
    {
        public EnumWeaponMaterialType type;
        public MaterialSlotRefs parentSlot;

        public bool isSelectable;

        public Renderer[] renderers;
        public Material originalMaterial;

        public EnumWeaponMaterialType[] compatibleMaterials;
    }

    public enum EnumWeaponMaterialType
    {
        None = 0,
        IceCrystal = 1,
        Bullet = 2,
        Balloon = 3,
        Arrow = 4,
        DarkSphere = 5,
    }
}