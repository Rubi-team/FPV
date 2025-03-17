using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemProximity {

    public class ButtonSphereInteractable : MonoBehaviour, IInteractable {


        [SerializeField] private MeshRenderer sphereMeshRenderer;
        [SerializeField] private Material blueMaterial;
        [SerializeField] private Material yellowMaterial;

        private bool isColorYellow;

        private void SetColorBlue() {
            sphereMeshRenderer.material = blueMaterial;
        }

        private void SetColorYellow() {
            sphereMeshRenderer.material = yellowMaterial;
        }

        private void ToggleColor() {
            isColorYellow = !isColorYellow;
            if (isColorYellow) {
                SetColorYellow();
            } else {
                SetColorBlue();
            }
        }

        public void PushButton() {
            ToggleColor();
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            PushButton();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string> {
                { IInteractable.InteractAction.Primary, "Push button" }
            };
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return interactAction == IInteractable.InteractAction.Primary;
        }

    }

}