using UnityEngine;

namespace _GAME.Player
{
    public class PlayerMoveLogic : MonoBehaviour
    {
        private PlayerFeature _playerFeature;
        private Level.LevelFeature _levelFeature;       
        
        private Transform ObjectToMove;
//        private float distance;

        private void Awake()
        {
            _playerFeature = GameFeature.PlayerFeature;
            _levelFeature = GameFeature.LevelFeature;
        }

        private void OnEnable()
        {
            _levelFeature.OnLevelLoaded += InitPlayer;
        }

        void Update()
        {
            if (_levelFeature.Level == null || _playerFeature.Player == null || !_playerFeature.Player.IsCanMove) return;

            //increase distance
            var speed = _playerFeature.PlayerSettings.PlayerMoveSpeed;


            if(_playerFeature.Player.MoveSpeed > 0)
            {
                speed = _playerFeature.Player.MoveSpeed;
            }

            _playerFeature.Player.MoveDistance +=  speed * Time.deltaTime;

            //calculate position and tangent
            Vector3 tangent;
            ObjectToMove.position = _levelFeature.Level.Path.CalcPositionAndTangentByDistance(_playerFeature.Player.MoveDistance, out tangent);
            var pos = ObjectToMove.localPosition;
            pos.y += 0.5f;
            ObjectToMove.localPosition = pos;
          
            var lookRot = Quaternion.LookRotation(tangent).eulerAngles;
            var rot = ObjectToMove.rotation.eulerAngles;

            ObjectToMove.rotation = Quaternion.Lerp(ObjectToMove.rotation, Quaternion.Euler(new Vector3(rot.x, lookRot.y, rot.z)), 10 * Time.deltaTime);
        } 
        
        private void InitPlayer(Level.LevelRefs lvl)
        {
            _playerFeature.Player.IsCanMove = false;
            ObjectToMove = _playerFeature.Player.transform;
            _playerFeature.Player.MoveDistance = 0;
        }
    }
}
