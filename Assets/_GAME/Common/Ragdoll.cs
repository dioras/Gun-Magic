using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Common
{
    [System.Serializable]
    public class Ragdoll : MonoBehaviour
    {
        public Collider MainCollider;
        public Rigidbody MainRigibody;
        public Animator Animator;

        public List<Rigidbody> Rigidbodies = new List<Rigidbody>();
        public List<Collider> Colliders = new List<Collider>();

        public ForceMode ForceMode { get; set; }
        public float ForcePower { get; set; }
        public Vector3 ForceDirection { get; set; }
        public bool IsTriggered { get; set; } = false;

        [ContextMenu("Init Ragdoll")]
        public void InitRagdoll()
        {
            Rigidbodies.Clear();
            Colliders.Clear();
           
            GetComponentsInChildren<Rigidbody>(Rigidbodies);
            GetComponentsInChildren<Collider>(Colliders);
           
            Animator = GetComponent<Animator>();

            if (Rigidbodies.Contains(MainRigibody)) Rigidbodies.Remove(MainRigibody);

            if (Colliders.Contains(MainCollider)) Colliders.Remove(MainCollider);

            for (int i = 0; i < Rigidbodies.Count; i++)
            {
                var rb = Rigidbodies[i];

                rb.isKinematic = true;
                rb.useGravity = false;
            }

            foreach (var col in Colliders)
            {
                col.isTrigger = true;
                col.enabled = false;
            }
        }

        [ContextMenu("Activate Ragdoll")]
        public void ActivateRagdoll()
        {
            if (MainCollider)
            {
                MainCollider.isTrigger = true;
                MainCollider.enabled = false;
            }

            if (MainRigibody)
            {
                MainRigibody.isKinematic = true;
                MainRigibody.useGravity = false;
            }

            foreach (var col in Colliders)
            {
                col.isTrigger = false;
                col.enabled = true;
            }

            Animator.enabled = false;

            foreach (var rb in Rigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                rb.AddForce(ForceDirection * ForcePower, ForceMode);
            }
        }

        public void AddForce()
        {
            foreach (var rb in Rigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                rb.AddForce(ForceDirection * ForcePower, ForceMode);
            }
        }

        [ContextMenu("DeActivate Ragdoll")]
        public void DeActivateRagdoll()
        {
            if (MainCollider)
            {
                MainCollider.isTrigger = false;
                MainCollider.enabled = true;
            }

            if (MainRigibody)
            {
                MainRigibody.isKinematic = false;
                MainRigibody.useGravity = true;
            }

            foreach (var col in Colliders)
            {
                col.isTrigger = true;
                col.enabled = false;
            }

            Animator.enabled = true;

            foreach (var rb in Rigidbodies)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        [ContextMenu("ActivateRagdollRigidbodies")]
        public void ActivateRagdollRigidbodies()
        {
            foreach (var rb in Rigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        [ContextMenu("DeActivateRagdollRigidbodies")]
        public void DeActivateRagdollRigidbodies()
        {
            foreach (var rb in Rigidbodies)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        [ContextMenu("ActivateRagdollColliders")]
        public void ActivateRagdollColliders()
        {
            foreach (var col in Colliders)
            {
                col.isTrigger = false;
                col.enabled = true;
            }
        }

        [ContextMenu("DeactivateRagdollColliders")]
        public void DeactivateRagdollColliders()
        {
            foreach (var col in Colliders)
            {
                col.isTrigger = true;
                col.enabled = false;
            }
        }
    }
}
