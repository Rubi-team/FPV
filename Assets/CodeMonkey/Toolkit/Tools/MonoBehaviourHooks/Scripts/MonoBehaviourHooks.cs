using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.TMonoBehaviourHooks {

    /// <summary>
    /// ** MonoBehaviour Hooks **
    /// 
    /// Simple utility class that allows you to attach Action Delegates to all the MonoBehaviour functions
    /// Useful when you want to add some extra logic on a certain event, like for debugging
    /// but you don't want to modify the original class
    /// </summary>
    public class MonoBehaviourHooks : MonoBehaviour {


        public Action onStartAction;
        public Action onDestroyAction;
        public Action onEnableAction;
        public Action onDisableAction;
        public Action onUpdateAction;
        public Action<Collision> onCollisionEnterAction;
        public Action<Collision> onCollisionExitAction;
        public Action<Collision> onCollisionStayAction;
        public Action<Collision2D> onCollisionEnter2DAction;
        public Action<Collision2D> onCollisionStay2DAction;
        public Action<Collision2D> onCollisionExit2DAction;
        public Action<Collider> onTriggerEnterAction;
        public Action<Collider> onTriggerStayAction;
        public Action<Collider> onTriggerExitAction;
        public Action<Collider2D> onTriggerEnter2DAction;
        public Action<Collider2D> onTriggerStay2DAction;
        public Action<Collider2D> onTriggerExit2DAction;
        public Action onBecameVisibleAction;
        public Action onBecameInvisibleAction;
        public Action onMouseDownAction;
        public Action onMouseUpAction;
        public Action onMouseUpAsButtonAction;
        public Action onMouseEnterAction;
        public Action onMouseExitAction;
        public Action onMouseDragAction;
        public Action onMouseOverAction;


        private void Start() {
            onStartAction?.Invoke();
        }

        private void OnDestroy() {
            onDestroyAction?.Invoke();
        }

        private void OnEnable() {
            onEnableAction?.Invoke();
        }

        private void OnDisable() {
            onDisableAction?.Invoke();
        }

        private void Update() {
            onUpdateAction?.Invoke();
        }

        private void OnCollisionEnter(Collision collision) {
            onCollisionEnterAction?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision) {
            onCollisionExitAction?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision) {
            onCollisionStayAction?.Invoke(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision2D) {
            onCollisionEnter2DAction?.Invoke(collision2D);
        }

        private void OnCollisionStay2D(Collision2D collision2D) {
            onCollisionStay2DAction?.Invoke(collision2D);
        }

        private void OnCollisionExit2D(Collision2D collision2D) {
            onCollisionExit2DAction?.Invoke(collision2D);
        }

        private void OnTriggerEnter(Collider collider) {
            onTriggerEnterAction?.Invoke(collider);
        }

        private void OnTriggerStay(Collider collider) {
            onTriggerStayAction?.Invoke(collider);
        }

        private void OnTriggerExit(Collider collider) {
            onTriggerExitAction?.Invoke(collider);
        }

        private void OnTriggerEnter2D(Collider2D collider2D) {
            onTriggerEnter2DAction?.Invoke(collider2D);
        }

        private void OnTriggerStay2D(Collider2D collider2D) {
            onTriggerStay2DAction?.Invoke(collider2D);
        }

        private void OnTriggerExit2D(Collider2D collider2D) {
            onTriggerExit2DAction?.Invoke(collider2D);
        }

        private void OnBecameVisible() {
            onBecameVisibleAction?.Invoke();
        }

        private void OnBecameInvisible() {
            onBecameInvisibleAction?.Invoke();
        }

        private void OnMouseDown() {
            onMouseDownAction?.Invoke();
        }

        private void OnMouseUp() {
            onMouseUpAction?.Invoke();
        }

        private void OnMouseUpAsButton() {
            onMouseUpAsButtonAction?.Invoke();
        }

        private void OnMouseEnter() {
            onMouseEnterAction?.Invoke();
        }

        private void OnMouseExit() {
            onMouseExitAction?.Invoke();
        }

        private void OnMouseDrag() {
            onMouseDragAction?.Invoke();
        }

        private void OnMouseOver() {
            onMouseOverAction?.Invoke();
        }

    }

}