using System;
using UnityEngine;

namespace _GAME.Common
{
    public class ObjectTapCatcher : MonoBehaviour
    {
        public bool IsActive;

        public event Action OnEnter;
        public event Action OnExit;
        public event Action<Transform> OnDown;
        public event Action OnUp;
        public event Action OnClick;

        private void OnMouseDown()
        {
            if (IsActive)
                OnDown?.Invoke(transform);
        }
        private void OnMouseUp()
        {
            if (IsActive)
                OnUp?.Invoke();
        }
        private void OnMouseEnter()
        {
            if (IsActive)
                OnEnter?.Invoke();
        }
        private void OnMouseExit()
        {
            if (IsActive)
                OnExit?.Invoke();
        }
        private void OnMouseUpAsButton()
        {
            if (IsActive)
                OnClick?.Invoke();
        }
    }
}