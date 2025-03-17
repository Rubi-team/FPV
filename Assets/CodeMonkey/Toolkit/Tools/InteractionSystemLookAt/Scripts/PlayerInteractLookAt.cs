using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

    /// <summary>
    /// ** Interaction System Look At **
    /// 
    /// General system for interacting with objects or NPCs
    /// Just implement the IInteractable interface and handle what Interaction means for that object
    /// It can be Talking for an NPC
    /// It can be Opening/Closing a Door
    /// It can be changing an object's Color
    /// It can be anything you want.
    /// 
    /// This one works based on Looking at the Interactable object
    /// </summary>
    public class PlayerInteractLookAt : MonoBehaviour {


        private const float SPHERE_CAST_RADIUS = .1f;
        private const float INTERACT_DISTANCE = 3f;


        [SerializeField] private bool testForInput = true;


        private Transform cameraMainTransform;


        private void Start() {
            cameraMainTransform = Camera.main.transform;
        }

        private void Update() {
            if (testForInput && Input.GetKeyDown(KeyCode.E)) {
                IInteractable interactable = GetInteractableObject();
                if (interactable != null) {
                    interactable.Interact(IInteractable.InteractAction.Primary, transform);
                }
            }
        }

        public IInteractable GetInteractableObject() {
            List<IInteractable> interactableList = new List<IInteractable>();
            List<Vector3> interactableHitPositionList = new List<Vector3>();
            RaycastHit[] raycastHitArray = Physics.SphereCastAll(cameraMainTransform.position, SPHERE_CAST_RADIUS, cameraMainTransform.forward, INTERACT_DISTANCE);
            foreach (RaycastHit raycastHit in raycastHitArray) {
                if (raycastHit.transform.TryGetComponent(out IInteractable interactable)) {
                    interactableList.Add(interactable);
                    interactableHitPositionList.Add(raycastHit.point);
                }
            }

            // Sort by closest
            IInteractable closestInteractable = null;
            Vector3 closestInteracableHitPosition = Vector2.zero;
            for (int i=0; i< interactableList.Count; i++) {
                IInteractable interactable = interactableList[i];
                Vector2 interactableHitPosition = interactableHitPositionList[i];
                if (closestInteractable == null) {
                    closestInteractable = interactable;
                    closestInteracableHitPosition = interactableHitPosition;
                } else {
                    if (Vector2.Distance(cameraMainTransform.position, interactableHitPosition) <
                        Vector2.Distance(cameraMainTransform.position, closestInteracableHitPosition)) {
                        // Closer
                        closestInteractable = interactable;
                        closestInteracableHitPosition = interactableHitPosition;
                    }
                }
            }

            return closestInteractable;
        }

    }

}