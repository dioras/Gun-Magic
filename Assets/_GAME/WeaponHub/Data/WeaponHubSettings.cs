using UnityEngine;

namespace _GAME
{
    [System.Serializable, CreateAssetMenu(fileName = "WeaponHubSettings", menuName = "GAME settings/WeaponHubSettings")]
    public class WeaponHubSettings : ScriptableObject
    {
        public WeaponMaterialRefs[] AvailableWeaponMaterials;
        public WeaponRefs[] AvailableWeapons;

        public EnumWeaponType TutorialWeaponType;

        public Vector3 CameraPosition;
        public Vector3 CameraRotation;

        [Header("Weapon Section")]
        public float WeaponRotationSpeed;

        [Header("Material Section")]
        public float MaterialSlotOffset;
        public float MaterialForgeOffset;
        public float MaterialRotationSpeed;

        public Material DisabledMaterial;
    }
}