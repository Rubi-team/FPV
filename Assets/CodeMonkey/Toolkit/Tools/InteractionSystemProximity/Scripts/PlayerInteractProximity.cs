using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemProximity {

    /// <summary>
    /// ** Interaction System Proxmity **
    /// 
    /// General system for interacting with objects or NPCs
    /// Just implement the IInteractable interface and handle what Interaction means for that object
    /// It can be Talking for an NPC
    /// It can be Opening/Closing a Door
    /// It can be changing an object's Color
    /// It can be anything you want.
    /// 
    /// This one works based on proximity, so it interacts with nearby objects
    /// </summary>
    public class PlayerInteractProximity : MonoBehaviour {


        private const float INTERACT_RANGE = 3f;


        private void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                IInteractable interactable = GetInteractableObject();
                if (interactable != null) {
                    interactable.Interact(IInteractable.InteractAction.Primary, transform);
                }
            }
        }

        public IInteractable GetInteractableObject() {
            List<IInteractable> interactableList = new List<IInteractable>();
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, INTERACT_RANGE);
            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent(out IInteractable interactable)) {
                    interactableList.Add(interactable);
                }
            }

            IInteractable closestInteractable = null;
            foreach (IInteractable interactable in interactableList) {
                if (closestInteractable == null) {
                    closestInteractable = interactable;
                } else {
                    if (Vector3.Distance(transform.position, interactable.GetTransform().position) <
                        Vector3.Distance(transform.position, closestInteractable.GetTransform().position)) {
                        // Closer
                        closestInteractable = interactable;
                    }
                }
            }

            return closestInteractable;
        }

    }

}