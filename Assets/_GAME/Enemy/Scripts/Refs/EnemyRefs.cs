using _GAME.Common;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _GAME.Enemy
{
    public class EnemyRefs : MonoBehaviour
    {
        public int IndexPrefab;

        public Transform HipsTransform;
        public Transform SpineTransform;

        public CollisionCatcher CollisionCatcher;
        public CollisionCatcher SpineCollisionCatcher;
        public CollisionCatcher HeadCollisionCatcher;

        public Ragdoll Ragdoll;

        public DesturctablePart DesturctablePart;

        public ForceParts[] AddforceRigibodies;

        public SkinnedMeshRenderer[] Renderers;
        public AudioSource AudioSource;

        public FreezeEffectRefs FreezeEffect;

        public float RotationSpeed = 1f;
        public float ForceForParts = 5;
        public bool IsForciblyStay = false;

        public float Height { get; set; }
        public bool CanMove { get; set; } = false;
        public bool IsAlive { get; set; } = true;


        [ContextMenu("Collect Renderers")]
        private void GetAllmeshRenderers()
        {
            Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }
    }

    [System.Serializable]
    public class DesturctablePart
    {
        public Transform Root;
        public Rigidbody[] Parts;
    }

    [System.Serializable]
    public class ForceParts
    {
        public Rigidbody Rigidbody;
        public float ForcePower;
        public bool IsUse = true;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyRefs))]
    public class EnemyRefsEditor : Editor
    {
        private EnemyRefs _enemyRefs;
        private List<Rigidbody> _bodiesForMoveFromRagdoll = new List<Rigidbody>();
        private List<Collider> _collidersForMoveFromRagdoll = new List<Collider>();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _enemyRefs = (EnemyRefs)target;

            if (GUILayout.Button("Collect Enemy"))
            {
                CollectEnemy();
                AddEnemyToPrefabsPreset();
            }
        }

        private void AddEnemyToPrefabsPreset()
        {
            var guids = AssetDatabase.FindAssets("t:EnemyPrefabsPreset", new[] { "Assets/_GAME/Enemy/Settings" });
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var preset = (EnemyPrefabsPreset)AssetDatabase.LoadAssetAtPath(path, typeof(EnemyPrefabsPreset));

            bool isFind = false;

            foreach (var enemy in preset.EnemyPrefabs)
            {
                if (enemy.IndexPrefab != 0 && enemy.IndexPrefab == _enemyRefs.IndexPrefab)
                {
                    isFind = true;
                    return;
                }
            }

            if (!isFind)
            {
                _enemyRefs.IndexPrefab = preset.EnemyPrefabs.Length;

                var newArray = new EnemyRefs[preset.EnemyPrefabs.Length + 1];

                for (int i = 0; i < preset.EnemyPrefabs.Length; i++)
                {
                    newArray[i] = preset.EnemyPrefabs[i];
                }

                newArray[preset.EnemyPrefabs.Length] = _enemyRefs;

                preset.EnemyPrefabs = newArray;

                EditorUtility.SetDirty(preset);
            }
        }

        private void CollectEnemy()
        {
            var enemyGo = _enemyRefs.gameObject;

            _enemyRefs.Renderers = enemyGo.GetComponentsInChildren<SkinnedMeshRenderer>();

            AddAudioSource();

            var ragdoll = enemyGo.GetComponentAsType<Ragdoll>();
            var animator = enemyGo.GetComponent<Animator>();

            _enemyRefs.HipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            _enemyRefs.SpineTransform = animator.GetBoneTransform(HumanBodyBones.Spine);
            _enemyRefs.Ragdoll = ragdoll;

            CollectRagdoll(ragdoll);

            AddCollisionCatchers(enemyGo, animator);

            AddRigidbodiesForForce(_enemyRefs, animator);

            UpdateEnemyLayers(ragdoll);

            UpdateFootsColliders(animator);

            RemoveBodiesFromRagdoll(ragdoll);

            AddDestractableParent();

            EditorUtility.SetDirty(_enemyRefs);
        }

        private void AddDestractableParent()
        {
            bool isFind = false;
            foreach (Transform child in _enemyRefs.transform)
            {
                if (child.gameObject.name != "DesturctablePartRoot") continue;
                else
                {
                    isFind = true;
                    _enemyRefs.DesturctablePart.Root = child;
                    return;
                }
            }

            if (!isFind)
            {
                var root = new GameObject("DesturctablePartRoot");
                root.SetActive(false);
                root.transform.SetParent(_enemyRefs.transform);

                _enemyRefs.DesturctablePart = new DesturctablePart();
                _enemyRefs.DesturctablePart.Root = root.transform;
            }
        }

        private void RemoveBodiesFromRagdoll(Ragdoll ragdoll)
        {
            foreach (var body in _bodiesForMoveFromRagdoll)
            {
                ragdoll.Rigidbodies.Remove(body);
            }

            foreach (var col in _collidersForMoveFromRagdoll)
            {
                ragdoll.Colliders.Remove(col);
            }
        }

        private void UpdateFootsColliders(Animator animator)
        {
            var col = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<Collider>();
            col.enabled = true;
            col.isTrigger = false;

            col = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<Collider>();
            col.enabled = true;
            col.isTrigger = false;
        }

        private void UpdateEnemyLayers(Ragdoll ragdoll)
        {
            foreach (var col in ragdoll.Colliders)
            {
                col.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }

            _enemyRefs.gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        private void CollectRagdoll(Ragdoll ragdoll)
        {
            ragdoll.MainRigibody = _enemyRefs.gameObject.GetComponentAsType<Rigidbody>();
            ragdoll.MainCollider = _enemyRefs.gameObject.GetComponentAsType<CapsuleCollider>();

            var capsule = (CapsuleCollider)ragdoll.MainCollider;
            capsule.radius = 0.71f;
            capsule.height = 2.829489f;
            capsule.direction = 1;

            var center = capsule.center;
            center.y = 1.8f;
            capsule.center = center;

            ragdoll.InitRagdoll();
            ragdoll.DeActivateRagdoll();

            ragdoll.MainCollider.isTrigger = true;
            ragdoll.MainRigibody.isKinematic = true;
        }

        private void AddAudioSource()
        {
            var audioSourceGo = _enemyRefs.gameObject.transform.GetComponentInChildren<AudioSource>().gameObject;

            if (!audioSourceGo) audioSourceGo = new GameObject("AudioSource");

            audioSourceGo.transform.SetParent(_enemyRefs.gameObject.transform);
            var audioSource = audioSourceGo.GetComponentAsType<AudioSource>();
            audioSource.playOnAwake = false;
            _enemyRefs.AudioSource = audioSource;
        }

        private void AddCollisionCatchers(GameObject enemyGo, Animator animator)
        {
            var collisionCatcher = enemyGo.GetComponentAsType<CollisionCatcher>();
            _enemyRefs.CollisionCatcher = collisionCatcher;

            _bodiesForMoveFromRagdoll.Clear();
            _collidersForMoveFromRagdoll.Clear();

            var spineGo = animator.GetBoneTransform(HumanBodyBones.Spine).gameObject;
            SetCollisionObjects(spineGo, 0.38f);

            _enemyRefs.SpineCollisionCatcher = spineGo.GetComponentAsType<CollisionCatcher>();

            var headGo = animator.GetBoneTransform(HumanBodyBones.Neck).gameObject;
            SetCollisionObjects(headGo, 0.35f, 0.5f);
            _enemyRefs.HeadCollisionCatcher = headGo.GetComponentAsType<CollisionCatcher>();
        }

        private void AddRigidbodiesForForce(EnemyRefs enemyRefs, Animator animator)
        {
            enemyRefs.AddforceRigibodies = new ForceParts[3];

            var head = animator.GetBoneTransform(HumanBodyBones.Head).gameObject;
            AddForcePart(enemyRefs.AddforceRigibodies, 0, head, 150);

            var pelvis = animator.GetBoneTransform(HumanBodyBones.Hips).gameObject;
            AddForcePart(enemyRefs.AddforceRigibodies, 1, pelvis, 25);

            var spine1 = animator.GetBoneTransform(HumanBodyBones.Spine).GetChild(0).gameObject;
            AddForcePart(enemyRefs.AddforceRigibodies, 2, spine1, 40);
        }

        private void AddForcePart(ForceParts[] forceParts, int index, GameObject target, float force)
        {
            var rb = target.GetComponent<Rigidbody>();

            forceParts[index] = new ForceParts
            {
                Rigidbody = rb,
                ForcePower = force,
                IsUse = true
            };
        }

        private void SetCollisionObjects(GameObject go, float radius, float yPos = 0)
        {
            var rigidbody = go.GetComponentAsType<Rigidbody>();
            rigidbody.isKinematic = true;
            var collider = go.GetComponentAsType<SphereCollider>();
            collider.radius = radius;
            collider.isTrigger = true;
            collider.enabled = true;
            var center = collider.center;
            center.y = yPos;
            collider.center = center;

            _bodiesForMoveFromRagdoll.Add(rigidbody);
            _collidersForMoveFromRagdoll.Add(collider);
        }
    }

    public static class Extensions
    {
        public static T GetComponentAsType<T>(this GameObject targetGo) where T : Component
        {
            var component = targetGo.GetComponent<T>() == null ?
                                 targetGo.AddComponent<T>() :
                                 targetGo.GetComponent<T>();
            return component as T;
        }
    }
#endif
}
