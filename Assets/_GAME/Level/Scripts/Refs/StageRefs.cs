using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _GAME.Level
{
    public class StageRefs : MonoBehaviour
    {
        public List<Enemy.EnemyRefs> EnemiesOnStage = new List<Enemy.EnemyRefs>();

        public int EnemiesCount;
#if UNITY_EDITOR
        public void AddEnemyInCollection(Enemy.EnemyRefs enemy)
        {
            EnemiesOnStage.Add(enemy);

            EnemiesCount = EnemiesOnStage.Count;
        }

        public void RemoveAllEnemies()
        {
            foreach (var enemy in EnemiesOnStage)
            {
                if (enemy == null) continue;
                if (enemy.gameObject == null) continue;
                DestroyImmediate(enemy.gameObject);
            }

            EnemiesOnStage.Clear();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StageRefs))]
    public class StageRefsEditor : Editor
    {
        private StageRefs _stageRefs;

        private void OnEnable()
        {
            _stageRefs = (StageRefs)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Collect Stage"))
            {
                CollectStageReferences();
            }


            if (GUILayout.Button("Show Enemies in stage"))
            {
                foreach (var enemy in _stageRefs.EnemiesOnStage)
                {
                    enemy.gameObject.SetActive(true);
                }
            }
        }

        private void CollectStageReferences()
        {
            _stageRefs.EnemiesOnStage = _stageRefs.gameObject.GetComponentsInChildren<Enemy.EnemyRefs>(false).ToList();

            _stageRefs.EnemiesCount = _stageRefs.EnemiesOnStage.Count;

            EditorUtility.SetDirty(_stageRefs);
        }
    }
#endif
}
