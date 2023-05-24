using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _GAME.Env
{
    public class ExplodeObjectsLogic : MonoBehaviour
    {
        private EnvFeature _envFeature;
        private Enemy.EnemyFeature _enemyFeature;
        private Level.LevelFeature _levelFeature;
        private VibroService.VibroFeature _vibroFeature;
        private Audio.AudioFeature _audioFeature;
        private Player.PlayerFeature _playerFeature;

        private List<ExplosionRefs> _explosions = new List<ExplosionRefs>();

        private bool _timeIsSlow = false;

        private void Awake()
        {
            _envFeature = FindObjectOfType<EnvFeature>();
            _enemyFeature = GameFeature.EnemyFeature;
            _levelFeature = GameFeature.LevelFeature;
            _vibroFeature = GameFeature.VibroFeature;
            _audioFeature = GameFeature.AudioFeature;
            _playerFeature = GameFeature.PlayerFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += (lvl) => InitExplosers();
        }

        private void InitExplosers()
        {
            _explosions = FindObjectsOfType<ExplosionRefs>(true).ToList();

            foreach (var exp in _explosions)
            {
                exp.CollisionCatcher.OnTriggerEnterEvent += (col) => Explode(col, exp);
            }
        }

        private void Explode(Collider col, ExplosionRefs exp)
        {
            if (!exp.IsTriggered)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
                    Activate(exp);
            }
        }

        private void Activate(ExplosionRefs exp)
        {
            exp.IsTriggered = true;

            if (!_timeIsSlow && !_playerFeature.Player.IsCanMove)
            {
                _timeIsSlow = true;

                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.85f, .1f).
                    onComplete += () =>
                    {
                        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, .5f);
                        _timeIsSlow = false;
                    };
            }

            var effect = Instantiate(_envFeature.ExplosionEffect);
            effect.transform.position = exp.Model.transform.position;
            effect.transform.rotation = exp.Model.transform.rotation;
            effect.Play(true);

            exp.DamagableTrigger.enabled = true;
            exp.DamagableTrigger.transform.SetParent(_levelFeature.Level.transform);
            exp.Model.SetActive(false);

            _vibroFeature.OnVibrateMedium?.Invoke();
            _audioFeature.PlaySound(Audio.EnumSound.BarrelExplosion, volume: 0.5f);

            DOTween.To(() => exp.DamagableTrigger.radius,
                       x => exp.DamagableTrigger.radius = x, 
                       exp.DamagableTriggerRadius, 0.3f).OnComplete(() => {
                           exp.gameObject.SetActive(false);
                           exp.DamagableTrigger.gameObject.SetActive(false);
                           });
        }
    }
}
