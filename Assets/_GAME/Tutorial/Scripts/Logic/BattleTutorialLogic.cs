using UnityEngine;

namespace _GAME.Tutorial
{
    public class BattleTutorialLogic : MonoBehaviour
    {
        private TutorialFeature _tutorialFeature;
        private Player.PlayerFeature _playerFeature;
        private Level.LevelFeature _levelFeature;
       
        private void Awake()
        {
            _tutorialFeature = FindObjectOfType<TutorialFeature>();
            _playerFeature = GameFeature.PlayerFeature;
            _levelFeature = GameFeature.LevelFeature;
          

            foreach (var tutor in _tutorialFeature.Tutorials)
            {
                foreach (var part in tutor.TutorialParts)
                {
                    foreach (var button in part.Targets)
                    {
                        button.onClick.AddListener(() => _playerFeature.OnTutorialShoot?.Invoke(button.transform.position));
                    }
                }
            }
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += ShowBattleTutorial;
            _tutorialFeature.OnShowBattleTutorial += Show;
        }

        [ContextMenu("Show Battle Tutorial")]
        private void ShowBattleTutorial(Level.LevelRefs level)
        {
            if (level.IsTutorial)
            {
                Show(level.tutorialIndex);
            }
        }

        private void Show(int index)
        {
            var tutor = _tutorialFeature.Tutorials[index];

            tutor.CurrentPartIndex = 0;

            HideAll(tutor);

            ShowBattleTutorialPart(tutor);

            _tutorialFeature.CurrentTutrialIndex++;
        }


        private void ShowBattleTutorialPart(TutorialUIRefs tutorial)
        {
            var tutPart = tutorial.TutorialParts[tutorial.CurrentPartIndex];

            foreach (var button in tutPart.Targets)
            {
                button.gameObject.SetActive(true);

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => HideBattleTutorialPart(tutorial));
                button.onClick.AddListener(() => _playerFeature.OnTutorialShoot?.Invoke(button.transform.position));
            }
        }

        private void HideBattleTutorialPart(TutorialUIRefs tutorial)
        {
            var tutPart = tutorial.TutorialParts[tutorial.CurrentPartIndex];

            foreach (var button in tutPart.Targets)
            {
                button.gameObject.SetActive(false);
            }

            if (tutorial.CurrentPartIndex + 1 == tutorial.TutorialParts.Length)
            {
                HideAll(tutorial);
            }
            else
            {
                tutorial.CurrentPartIndex++;
                this.DelayedCall(0.7f, () => ShowBattleTutorialPart(tutorial));
            }
        }

        private void HideAll(TutorialUIRefs tutpart)
        {
            foreach (var part in tutpart.TutorialParts)
            {
                foreach (var but in part.Targets)
                {
                    but.gameObject.SetActive(false);
                }
            }
        }
    }
}
