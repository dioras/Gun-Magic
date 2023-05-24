using _GAME.Level;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Env
{
    public class HelicopterFlyLogic : MonoBehaviour
    {
        private LevelFeature _levelFeature;
        private LevelUIView.LevelUIFeature _levelUIFeature;
        private Audio.AudioFeature _audioFeature;

        private List<Transform> _platforms = new List<Transform>();

        private void Awake()
        {
            _levelFeature = GameFeature.LevelFeature;
            _levelUIFeature = GameFeature.LevelUIFeature;
            _audioFeature = GameFeature.AudioFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += UpdateHelicopterVisible;
            _levelFeature.OnStartLevel += UpdateHelicopterPath;
            _levelFeature.OnStartLevel += UpdateHelicoptersAudioSourceState;
        }

        private void UpdateHelicopterVisible(LevelRefs level)
        {
            if (!level.NeedHelicopter)
            {
                _levelFeature.LevelBase.Helicopter.gameObject.SetActive(false);
            }
        }

        private void UpdateHelicoptersAudioSourceState()
        {
            var audioSources = _levelFeature.Level.gameObject.GetComponentsInChildren<AudioSource>();

            foreach (var source in audioSources)
            {
                source.enabled = _audioFeature.IsSoundOn;
            }
        }

        private void UpdateHelicopterPath()
        {
            _platforms.Clear();

            if (!_levelFeature.LevelBase.Helicopter.gameObject.activeSelf) return;
            
            foreach (Transform child in _levelFeature.Level.transform)
            {
                if (child.gameObject.name.Contains("Platf")) _platforms.Add(child);
            }

            var finish = _platforms[0];
            _platforms.Remove(finish);
            _platforms.Add(finish);

            var pathPointsCount = _platforms.Count + 1;
            _levelFeature.LevelBase.Helicopter.PathPoints = new Vector3[pathPointsCount];

            var offset = _levelFeature.LevelBase.Helicopter.AxisXOffset;
            var posxOffset = Random.Range(-offset, offset);

            for (int i = 0; i < _platforms.Count; i++)
            {
                var pos = _platforms[i].transform.position;
                pos.y += _levelFeature.LevelBase.Helicopter.FlyHeight;
                pos.x = posxOffset;

                _levelFeature.LevelBase.Helicopter.PathPoints[i] = pos;
            }

            var lastPointPositionZ = _platforms[_platforms.Count - 1].transform.position.z 
                                        + _levelFeature.LevelBase.Helicopter.FlyHeight * 2;

            Vector3 finishPoint = new Vector3(0, -8, lastPointPositionZ);

            _levelFeature.LevelBase.Helicopter.PathPoints[pathPointsCount - 1] = finishPoint;

            HelicopterMove();
        }

        private void HelicopterMove()
        {
            var helicopter = _levelFeature.LevelBase.Helicopter;
            var helicopterTransform = helicopter.transform;
            var path = _levelFeature.LevelBase.Helicopter.DoTweenPath;

            var delay = _platforms.Count < 5 == true ? .5f : path.delay;

            helicopterTransform.DOPath(helicopter.PathPoints, path.duration, path.pathType, path.pathMode).SetDelay(delay)
                .OnComplete(() => path.onComplete.Invoke());
        }
    }
}
