using _GAME.LevelUIView;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Audio
{
    public class GameSoundsLogic : MonoBehaviour
    {
        private AudioFeature _audio;
        private GameFeature _gameFeature;
        private LevelUIFeature _levelUiFeature;

        private void Awake()
        {
            _audio = GameFeature.AudioFeature;
            _gameFeature = GameFeature.Instance;
            _levelUiFeature = GameFeature.LevelUIFeature;

            _gameFeature.OnStartGame += InitButtonSounds;
        }

        private void InitButtonSounds()
        {
            var uiButtons = FindObjectsOfType<Button>(true).ToList();

            uiButtons.Remove(_levelUiFeature.TapToStartView.UIButton);

            foreach (var btn in uiButtons)
                btn.onClick.AddListener(() => _audio.PlaySound(EnumSound.ButtonClicked));
        }
    }
}