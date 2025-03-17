using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ContainerBox : MonoBehaviour, IInteractable {


        public enum State {
            Ground,
            PickedUp
        }


        [SerializeField] private ObjectType objectType;
        [SerializeField] private TextMeshPro amountTextMesh;


        private State state = State.Ground;
        private BoxCollider boxCollider;
        private int amount = 15;


        private void Awake() {
            boxCollider = GetComponent<BoxCollider>();
            amountTextMesh.text = amount.ToString();
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string>{
                { IInteractable.InteractAction.PickUpBox, "Pick up" }
            };
        }

        public ObjectType GetObjectType() {
            return objectType;
        }

        public Transform GetTransform() {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            return
                interactAction == IInteractable.InteractAction.PickUpBox ||
                interactAction == IInteractable.InteractAction.DropBox;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            if (!PlayerShopSimulator.Instance.IsCarryingContainerBox()) {
                // Not yet carrying a box, can carry
                PlayerShopSimulator.Instance.SetCarryingContainerBox(this);
            }
        }

        public bool CanAddAmount(ObjectType objectType) {
            if (GetObjectType() == objectType) {
                return true;
            } else {
                return false;
            }
        }

        public void AddAmount() {
            amount++;
            amountTextMesh.text = amount.ToString();
        }

        public bool CanRemoveAmount() {
            return amount > 0;
        }

        public void RemoveAmount() {
            amount--;
            amountTextMesh.text = amount.ToString();
        }

        public void SetState(State state) {
            this.state = state;

            switch (state) {
                case State.Ground:
                    boxCollider.enabled = true;
                    break;
                case State.PickedUp:
                    boxCollider.enabled = false;
                    break;
            }
        }

    }

}