using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeMonkey.Toolkit.TPointerHooks {

    /// <summary>
    /// ** Pointer Hooks **
    /// 
    /// Simple utility class that allows you to attach Action Delegates to all the IPointer functions
    /// Useful when you want to add some extra logic on a certain event, like IPointerDown
    /// but you don't want to modify the original class
    /// </summary>
    public class PointerHooks : MonoBehaviour,
        IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler,
        IPointerExitHandler, IPointerMoveHandler, IPointerUpHandler {


        public Action<PointerEventData> onPointerDownAction;
        public Action<PointerEventData> onPointerClickAction;
        public Action<PointerEventData> onPointerEnterAction;
        public Action<PointerEventData> onPointerExitAction;
        public Action<PointerEventData> onPointerMoveAction;
        public Action<PointerEventData> onPointerUpAction;


        public void OnPointerDown(PointerEventData eventData) {
            onPointerDownAction?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData) {
            onPointerClickAction?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            onPointerEnterAction?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData) {
            onPointerExitAction?.Invoke(eventData);
        }

        public void OnPointerMove(PointerEventData eventData) {
            onPointerMoveAction?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            onPointerUpAction?.Invoke(eventData);
        }

    }

}