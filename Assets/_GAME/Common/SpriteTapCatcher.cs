using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _GAME.Common
{
    /// <summary>
    /// Класс отлавливает нажатие на спрайт (реализует интерфейс ITapCatcher).
    /// </summary>
    public class SpriteTapCatcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] bool showLog;

        public event Action OnEnter;
        public event Action OnExit;
        public event Action OnDown;
        public event Action OnUp;
        public event Action OnClick;

        public bool Active
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        private bool visible;
        public bool Visible
        {
            get => visible;
            set
            {
                var color = GetComponent<Image>().color;
                color.a = value ? 0.5f : 0.01f;
                GetComponent<Image>().color = color;
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (showLog)
                Debug.Log("Enter " + gameObject.name);
            OnEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (showLog)
                Debug.Log("Exit " + gameObject.name);
            OnExit?.Invoke();
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (showLog)
                Debug.Log("Down " + gameObject.name);
            OnDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (showLog)
                Debug.Log("Up " + gameObject.name);
            OnUp?.Invoke();
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (showLog)
                Debug.Log("Click " + gameObject.name);
            OnClick?.Invoke();
        }
    }
}
