using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ScanObjectInteractable : MonoBehaviour, IInteractable {


        public bool CanDoInteractAction(IInteractable.InteractAction interactAction) {
            if (Checkout.Instance.HasObjectWaitingToScan()) {
                return interactAction == IInteractable.InteractAction.ScanObject;
            } else {
                return false;
            }
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            if (Checkout.Instance.HasObjectWaitingToScan()) {
                return new Dictionary<IInteractable.InteractAction, string> {
                { IInteractable.InteractAction.ScanObject, "Scan Object" }
            };
            } else {
                return null;
            }
        }

        public Transform GetTransform() {
            return transform;
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            if (interactAction == IInteractable.InteractAction.ScanObject) {
                Checkout.Instance.ScanObject();
            }
        }

    }

}