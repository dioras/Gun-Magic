using UnityEngine;
using _GAME.Level;

namespace _GAME.Enemy
{
    public class EnemyChangeColorLogic : MonoBehaviour
    {
        private EnemyFeature _enemyFeature;
        private VibroService.VibroFeature _vibroFeature;
        private Audio.AudioFeature _audioFeature;
        private LevelFeature _levelFeature;

        private void Awake()
        {
            _enemyFeature = GameFeature.EnemyFeature;
            _vibroFeature = GameFeature.VibroFeature;
            _audioFeature = GameFeature.AudioFeature;
            _levelFeature = GameFeature.LevelFeature;
        }

        private void OnEnable()
        {
            _enemyFeature.OnEnemyDestruct += Destruct;
            _enemyFeature.OnEnemyDieWithoutDestract += ChangeColor;
        }

        private void ChangeColor(EnemyRefs enemy)
        {
            foreach (var renderer in enemy.Renderers)
            {
                renderer.material.SetColor("_BaseColor", _enemyFeature.EnemySettings.ColorOnDie);
                
                if (enemy.SpineTransform)
                {
                    var effect = Instantiate(_enemyFeature.EnemySettings.EnemyEffectsPreset.ElectroEffect);
                    effect.transform.position = enemy.SpineTransform.position;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                this.DelayedCall(i * .05f, () =>
                {
                    _vibroFeature.OnVibrateMedium?.Invoke();
                });
            }

            this.DelayedCall(1f, () => enemy.gameObject.SetActive(false));
        }

        private void Destruct(EnemyRefs enemy, bool getHips = true)
        {
            if (getHips)
            {
                enemy.DesturctablePart.Root.SetParent(_levelFeature.Level.transform);

                if (enemy.HipsTransform)
                {
                    var pos = enemy.HipsTransform.position; //.Ragdoll.Animator.GetBoneTransform(HumanBodyBones.Hips).position;
                    var rot = enemy.HipsTransform.rotation; // Ragdoll.Animator.GetBoneTransform(HumanBodyBones.Hips).rotation;
                    enemy.DesturctablePart.Root.position = pos;
                    enemy.DesturctablePart.Root.rotation = rot;
                }
            }
            else
            {
                enemy.DesturctablePart.Root.SetParent(_levelFeature.Level.transform);
                _enemyFeature.OnSetDestructablePartsPosition?.Invoke(enemy);
            }


            enemy.gameObject.SetActive(false);
            enemy.DesturctablePart.Root.gameObject.SetActive(true);

            if (enemy.SpineTransform != null)
            {
                var effect = Instantiate(_enemyFeature.EnemySettings.EnemyEffectsPreset.BoomEnemyEffect);
                effect.transform.position = enemy.SpineTransform.position;
            }

            _vibroFeature.OnVibrateHard?.Invoke();

            foreach (var part in enemy.DesturctablePart.Parts)
            {
                part.transform.rotation = Random.rotation;
                part.AddForce(part.transform.forward * enemy.ForceForParts, ForceMode.Impulse);
            }
        }
    }
}