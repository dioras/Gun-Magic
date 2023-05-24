using UnityEngine;

namespace _GAME
{
    public class WeaponHubRefs : MonoBehaviour
    {
        public Camera HubCamera;
        public Transform CameraLookAtAnchor;
        public Transform CameraMoveAnchor;

        [Header("Weapon Section")]
        public Transform WeaponHolder;
        public ParticleSystem CraftEffect;

        public GameObject[] PillarLightnings;
        public GameObject[] TableLightnings;

        public Transform TableTransform;
        public Transform MonitorTransform;

        [Header("Material Section")]
        public MaterialSlotRefs[] Slots;
        public ForgeRefs Forge1;
        public ForgeRefs Forge2;

        public Transform MergeEpicenter;
        public ParticleSystem MergeEffect;
    }
}