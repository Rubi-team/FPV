using UnityEngine;

namespace CodeMonkey.Toolkit.TFirstPersonController {

    /// <summary>
    /// ** First Person Controller **
    /// 
    /// Simple Character Controller in First Person
    /// Use the Mouse to look around with the Camera
    /// Use WASD to Move around, Space to Jump
    /// </summary>
    public class FirstPersonController : MonoBehaviour {


        [SerializeField] private Camera playerCamera;
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float moveSpeed = 10f;


        private CharacterController characterController;
        private float cameraVerticalAngle;
        private float verticalMovement;


        private void Awake() {
            characterController = GetComponent<CharacterController>();
            LockMouse();

            if (playerCamera == null) {
                Debug.LogError("PlayerCamera field needs to be assigned! Drag the camera reference which should be a child object of the Player object, check the Prefab");
            }
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

            Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
            moveVector.y = verticalMovement;

            // Transform moveVector from localSpace into worldSpace
            moveVector = transform.TransformVector(moveVector);

            characterController.Move(moveVector * moveSpeed * Time.deltaTime);

            
            // Handle Mouse Look Around
            Vector2 mouseVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            // Horizontal Camera rotation
            float rotationSpeed = 1f * sensitivity;
            transform.Rotate(new Vector3(0f, mouseVector.x * rotationSpeed, 0f), Space.Self);

            // Vertical Camera rotation
            // Add vertical inputs to the camera's vertical angle
            cameraVerticalAngle += -mouseVector.y * rotationSpeed;

            // Limit the camera's vertical angle to min/max so it doesn't flip over
            cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

            // Apply the vertical angle as a local rotation
            playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

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