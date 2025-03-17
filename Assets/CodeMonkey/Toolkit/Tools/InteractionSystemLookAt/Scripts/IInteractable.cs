using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

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