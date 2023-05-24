using UnityEngine;

namespace _GAME.Scenes
{
	public class GlobalGameObject : MonoBehaviour
    {
        private void Awake()
        {
            var objectsWithTheSameTag = GameObject.FindGameObjectsWithTag(gameObject.tag);
            if (objectsWithTheSameTag.Length >= 2)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
    }
}