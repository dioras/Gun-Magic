using _GAME.Level;
using _GAME.LevelUIView;
using DG.Tweening;
using UnityEngine;

namespace _GAME
{
    public class TransitionToLevelLogic : MonoBehaviour
    {
        private GameFeature _gameFeature;
        private WeaponHubFeature _weaponHubFeature;
        private LevelUIFeature _levelUIFeature;
        private LevelFeature _levelFeature;
        private void OnEnable()
        {
            _gameFeature = FindObjectOfType<GameFeature>();
            _weaponHubFeature = FindObjectOfType<WeaponHubFeature>();
            _levelUIFeature = FindObjectOfType<LevelUIFeature>();
            _levelFeature = FindObjectOfType<LevelFeature>();

            _gameFeature.OnTransitionToLevel += TransitionToLevel;
        }

        private void TransitionToLevel()
        {
            _levelUIFeature.WhiteOverlay.DOFade(1, 1f).OnComplete(() =>
            {
                _weaponHubFeature.Refs.HubCamera.gameObject.SetActive(false);
                _levelUIFeature.WhiteOverlay.DOFade(0, 1f);

                _levelFeature.OnStartLevel?.Invoke();
            });
        }
    }
}