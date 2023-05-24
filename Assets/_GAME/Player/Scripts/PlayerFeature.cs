using UnityEngine;

namespace _GAME.Player
{
    public class PlayerFeature : MonoBehaviour
    {
        public PlayerSettings PlayerSettings;
        public Shop.ShopElements ShopPreset;

        public PlayerRefs Player;
        public ProgressBarRefs ProgressBar;

        public bool CanShoot = false;
        public bool IsMoving = false;
        public int CurrentPlatform = 1;

        public BulletRefs ActiveBullet;

        public float DelayToMoveNextStage = 1;
        public float DelayBeforeShoot = 0.25f;

        public int ShopId = 1;
        public string DeathReason;

        [HideInInspector] public int Shoot;

        public System.Action<Vector3> OnTutorialShoot;
        public System.Action<int> OnCameToNextPlatform;
        public System.Action OnPlayerMoveStart;
        public System.Action OnPlayerMoveEnd;
        private void Awake()
        {
            Shoot = Animator.StringToHash(PlayerSettings.AnimationShootKey);
        }
    }
}
