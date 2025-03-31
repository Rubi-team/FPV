using System.Collections.Generic;
using UnityEngine;

namespace FPV
{
    public interface IInteractable {

        public enum InteractAction {
            Primary,
            Secondary,
        }

        public bool CanDoInteractAction(InteractAction interactAction);

        public void Interact(InteractAction interactAction, Transform interactorTransform);

        public Dictionary<InteractAction, string> GetInteractTextDictionary();

        public Transform GetTransform();

    }

}
