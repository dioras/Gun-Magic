using _GAME.Audio;
using DG.Tweening;
using UnityEngine;

namespace _GAME.Scenes
{
    public class GlobalManagementFeature : MonoBehaviour
    {
        [SerializeField] private bool _useSafeMode = true;
        [SerializeField] LogBehaviour _logBehaviour = LogBehaviour.ErrorsOnly;
        
        
        private AudioFeature _audioFeature;
        private Preload.PreloadFeature _preloadFeature;

        private void Awake()
        {
            _preloadFeature = FindObjectOfType<Preload.PreloadFeature>();
            _audioFeature = FindObjectOfType<AudioFeature>();
        }

        private void OnEnable()
        {
            if (_preloadFeature)
            {
                _preloadFeature.OnGameSceneLoaded += () =>
                {
                    if (_audioFeature == null)
                    {
                        _audioFeature = FindObjectOfType<AudioFeature>();
                        _audioFeature.Init();
                    }
                };
            }
        }
        private void Start()
        {
            DOTween.Init(true, _useSafeMode, _logBehaviour).SetCapacity(1000, 1000);

            if(_audioFeature) _audioFeature.Init();
        }
    }
}