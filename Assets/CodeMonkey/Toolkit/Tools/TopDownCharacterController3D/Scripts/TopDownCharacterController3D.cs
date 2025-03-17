using UnityEngine;

namespace CodeMonkey.Toolkit.TTopDownCharacterController3D {

    /// <summary>
    /// ** Top Down Character Controller 3D **
    /// 
    /// Simple Character Controller 3D in Top Down perspective (no gravity)
    /// Use WASD to Move around
    /// </summary>
    public class TopDownCharacterController3D : MonoBehaviour {


        private Transform mainCameraTransform;
        private CharacterController characterController;
        private float verticalMovement;


        private void Awake() {
            characterController = GetComponent<CharacterController>();
            LockMouse();
        }

        private void Start() {
            mainCameraTransform = Camera.main.transform;

        }
        private void Update() {
            // Handle Gravity Vertical movement
            if (characterController.isGrounded && verticalMovement < 0) {
                verticalMovement = 0f;
            } else {
                // Not grounded
                float gravity = 7.8f;
                verticalMovement -= Time.deltaTime * gravity;
            }

            // Get Input vector from Keyboard Inputs
            Vector2 inputVector = new Vector2(0, 0);
            if (Input.GetKey(KeyCode.W)) {
                inputVector.y = +1;
            }
            if (Input.GetKey(KeyCode.S)) {
                inputVector.y = -1;
            }
            if (Input.GetKey(KeyCode.A)) {
                inputVector.x = -1;
            }
            if (Input.GetKey(KeyCode.D)) {
                inputVector.x = +1;
            }
            inputVector = inputVector.normalized;

            Vector3 moveVector = mainCameraTransform.forward * inputVector.y + mainCameraTransform.right * inputVector.x;
            moveVector.y = verticalMovement;

            float moveSpeed = 10f;
            characterController.Move(moveVector * moveSpeed * Time.deltaTime);

            // Test for Input and if character can Jump
            if (characterController.isGrounded && Input.GetKey(KeyCode.Space)) {
                float jumpAmount = 1.8f;
                verticalMovement = jumpAmount;
            }
        }

        public void UnlockMouse() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void LockMouse() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Disable() {
            this.enabled = false;
        }

        public void Enable() {
            this.enabled = true;
        }

    }

}