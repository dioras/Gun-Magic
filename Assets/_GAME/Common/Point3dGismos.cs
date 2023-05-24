using UnityEngine;

namespace _GAME.Common
{
    public class Point3dGismos : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.2f); 
        }
    }
}