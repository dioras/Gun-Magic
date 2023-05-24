using _GAME.Common;
using UnityEngine;

namespace _GAME.Player
{
    public class PlayerRefs : MonoBehaviour
    {
        [Header("References")]
        public Transform ShootPoint;
        public CollisionCatcher CollisionCatcher;
        public Transform[] BalloonPossibleTrajectories;

        [Header("RunTime Fields")]
        public bool IsAlive = true;
        public bool IsCanMove = false;
        public bool IsImmortal = false;
        public float MoveSpeed = 0f;
        public float MoveDistance;
    }
}
