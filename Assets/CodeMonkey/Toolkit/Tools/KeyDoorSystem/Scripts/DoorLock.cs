using UnityEngine;

namespace CodeMonkey.Toolkit.TKeyDoorSystem {

    /// <summary>
    /// Added to Door, open with this Key reference
    /// </summary>
    public class DoorLock : MonoBehaviour {


        [Header("Door Lock")]
        [Tooltip("The Key that opens this Door")]
        [SerializeField] private Key key;

        [Tooltip("Remove the Key from the Holder after using it to open this Door?")]
        [SerializeField] private bool removeKeyOnUse;


        private Animator animator;


        private void Awake() {
            // Cache Animator Component
            animator = GetComponent<Animator>();
        }

        public void OpenDoor() {
            // Play Open Door Animation
            animator.SetTrigger("Open");
        }

        public void CloseDoor() {
            // Play Close Door Animation
            animator.SetTrigger("Close");
        }

        public bool ShouldRemoveKeyOnUse() {
            return removeKeyOnUse;
        }

        public Key GetKey() {
            return key;
        }

    }

}