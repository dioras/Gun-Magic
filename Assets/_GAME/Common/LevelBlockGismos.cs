using UnityEngine;

namespace _GAME.Common
{
    public class LevelBlockGismos : MonoBehaviour
    {
        [SerializeField] private Vector3 blockSize;
        [SerializeField] private Vector3 centerOffset;
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + centerOffset, blockSize); 
        }
    }
}
