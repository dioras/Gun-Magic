using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Env
{
    public class BallsParentRefs : MonoBehaviour
    {
        public List<BallRefs> Balls = new List<BallRefs>();
        public float BallForce = 5;
        public float Radius = 0.15f;

        public Rigidbody Rigidbody;
        public Collider Collider;
        public Transform ShootPoint;
        public Transform[] ShootPoints;
    }
}
