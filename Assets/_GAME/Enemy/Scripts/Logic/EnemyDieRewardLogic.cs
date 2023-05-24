using UnityEngine;

namespace _GAME.Enemy
{
    public class EnemyDieRewardLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private Wallet.WalletFeature _walletFeature;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _walletFeature = GameFeature.WalletFeature;
        }

        private void OnEnable()
        {
            _enemyFeature.OnEnemyDie += Reward;
        }

        private void Reward(EnemyRefs enemy)
        {
            _walletFeature.CurrentReward += 30;
        }
    }
}
