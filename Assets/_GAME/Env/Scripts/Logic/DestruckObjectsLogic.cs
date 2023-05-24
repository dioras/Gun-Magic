using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _GAME.Env
{
    public class DestruckObjectsLogic : MonoBehaviour
    {
        private EnvFeature _envFeature;
        private Level.LevelFeature _levelFeature;
        private Enemy.EnemyFeature _enemyFeature;
        private Audio.AudioFeature _audioFeature;

        private List<DestructibleRefs> _destracters = new List<DestructibleRefs>();

        private void Awake()
        {
            _envFeature = FindObjectOfType<EnvFeature>();
            _levelFeature = GameFeature.LevelFeature;
            _enemyFeature = GameFeature.EnemyFeature;
            _audioFeature = GameFeature.AudioFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += (lvl) => InitDestruckters();
        }

        private void InitDestruckters()
        {
            _destracters = FindObjectsOfType<DestructibleRefs>(true).ToList();

            foreach (var dest in _destracters)
            {
                dest.CollisionCatcher.OnTriggerEnterEvent += (col) => Explode(col, dest);
            }
        }

        private void Explode(Collider col, DestructibleRefs dest)
        {
            if (1 << col.gameObject.layer == _envFeature.DamagableLayer ||
                1 << col.gameObject.layer == _enemyFeature.EnemySettings.EnemyLayer)
            {
                if (!dest.IsTriggered)
                {
                    Destruct(dest);
                }
            }
        }

        private void Destruct(DestructibleRefs dest)
        {
            dest.IsTriggered = true;
            dest.Model.SetActive(false);

            _audioFeature.PlaySound(Audio.EnumSound.CrateDestruct, volume: .1f);

            dest.PartsParent.gameObject.SetActive(true);
            dest.PartsParent.SetParent(_levelFeature.Level.transform);

            StartCoroutine(DestructCor(dest.Parts));

            dest.gameObject.SetActive(false);
            dest.MainCollider.enabled = false;
        }

        private IEnumerator DestructCor(List<Rigidbody> parts)
        {
            foreach (var rb in parts)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                rb.transform.rotation = Random.rotation;

                rb.AddForce(rb.transform.forward * _envFeature.DestructableExplosionForce, ForceMode.Impulse);

                Destroy(rb.gameObject, 3);

                yield return null;
            }
        }
    }
}
