using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

    public class DoorInteractable : MonoBehaviour, IInteractable {


        private Animator animator;
        private bool isOpen;


        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public void ToggleDoor() {
            isOpen = !isOpen;
            if (isOpen) {
                animator.SetTrigger("Open");
            } else {
                animator.SetTrigger("Close");
            }
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            ToggleDoor();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string> {
                { IInteractable.InteractAction.Primary, "Open/Close Door" }
            };
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return interactAction == IInteractable.InteractAction.Primary;
        }

        public Transform GetTransform() {
            return transform;
        }

    }

}