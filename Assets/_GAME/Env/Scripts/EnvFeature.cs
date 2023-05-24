using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _GAME.Env
{
    public class EnvFeature : MonoBehaviour
    {
        public LayerMask DamagableLayer;
        
        public System.Action<ExplosionRefs> OnExplode;

        public float DestructableExplosionForce = 10;
        public float DestructableExplosionRadius = 2;

        public ParticleSystem ExplosionEffect;

        private async void Awake()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("GrenadeExplosionFire");
            await handle.Task;

            ExplosionEffect = handle.Result.GetComponent<ParticleSystem>();
        }
    }
}
