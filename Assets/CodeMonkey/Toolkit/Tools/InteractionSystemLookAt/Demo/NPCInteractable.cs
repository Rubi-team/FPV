using CodeMonkey.Toolkit.TChatBubble3D;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TInteractionSystemLookAt {

    public class NPCInteractable : MonoBehaviour, IInteractable {


        private ChatBubble3D chatBubble3D;


        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform) {
            chatBubble3D?.DestroySelf();    
            chatBubble3D = ChatBubble3D.Create(transform.transform, new Vector3(.3f, 2f, 0f), ChatBubble3D.IconType.Happy, "Hello there!", .1f);
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary() {
            return new Dictionary<IInteractable.InteractAction, string> {
                { IInteractable.InteractAction.Primary, "Talk" }
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