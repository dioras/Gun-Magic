using UnityEngine;
using _GAME.Enemy;

namespace _GAME.Player
{
    public class ProgreesBarLogic : MonoBehaviour
    {
        private PlayerFeature _playerFeature;
        private Level.LevelFeature _levelFeature;
        private Enemy.EnemyFeature _enemyFeature;

        private float _fillValue;

        private void Awake()
        {
            _playerFeature = GameFeature.PlayerFeature;
            _levelFeature = GameFeature.LevelFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature.OnGetLevelProgress += GetProgress;
        }

        private int GetProgress()
        {
            return (int)(_playerFeature.ProgressBar.ProgressImage.fillAmount * 100);
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += InitProgressBar;
            _enemyFeature.OnEnemyDie += UpdateProgressBar;

        }

        private void UpdateProgressBar(EnemyRefs enemy)
        {
            _playerFeature.ProgressBar.ProgressImage.fillAmount += _fillValue;
        }

        private void InitProgressBar(Level.LevelRefs level)
        {
            _playerFeature.ProgressBar.ProgressImage.fillAmount = 0;

            int counter = 0;

            foreach (var stage in level.Stages)
            {
                counter += stage.EnemiesOnStage.Count;
            }

            _fillValue = (float)1 / counter;

            _playerFeature.ProgressBar.CurrentLevel.SetText(_levelFeature.CurrentLevel.ToString());
            _playerFeature.ProgressBar.NextLevel.SetText((_levelFeature.CurrentLevel + 1).ToString());
        }
    }
}
