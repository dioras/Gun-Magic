using System.Collections.Generic;
using UnityEngine;
using _GAME.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _GAME.Env
{
    public class DestructibleRefs : MonoBehaviour
    {
        public GameObject Model;
        public Transform PartsParent;
        public CollisionCatcher CollisionCatcher;
        public Rigidbody Rigidbody;
        public Collider MainCollider;
        public bool IsTriggered = false;

        public List<Rigidbody> Parts = new List<Rigidbody>();
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(DestructibleRefs))]
    public class DestructibleRefsEditor : Editor
    {
        private DestructibleRefs _destructibleRefs;

        public override void OnInspectorGUI()
        {
            _destructibleRefs = (DestructibleRefs)target;

            base.OnInspectorGUI();

            if(GUILayout.Button("Collect Rigibodies"))
            {
                _destructibleRefs.PartsParent.GetComponentsInChildren<Rigidbody>(_destructibleRefs.Parts);
            }
        }
    }
#endif
}
