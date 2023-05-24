using System;
using System.Collections.Generic;
using UnityEngine;

namespace _GAME
{
    public class WeaponHubFeature : MonoBehaviour
    {
        public Action OnShowCraftEffects;
        public Action OnCreateWeapon;
        public Action OnMoveIntoPortal;
        public Action OnReloadHub;

        public WeaponHubSettings Settings;
        public WeaponHubRefs Refs;

        public List<WeaponMaterialRefs> SlotMaterials;
        public List<EnumWeaponMaterialType> SelectedMaterials;

        public WeaponRefs CraftedWeapon;
    }
}