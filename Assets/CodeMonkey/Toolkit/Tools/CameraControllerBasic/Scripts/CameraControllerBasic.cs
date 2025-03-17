using UnityEngine;

namespace CodeMonkey.Toolkit.TCameraControllerBasic {

    /// <summary>
    /// ** Camera Controller Basic **
    /// 
    /// Very simple Camera Controller, great for quickly prototyping
    /// Just attach this script, then use WASD to move the camera, and QE to rotate.
    /// 
    /// Make sure this script is on a Transform that has the Camera as a Child object
    /// This Transform will move and rotate, which will also move and rotate the child Camera
    /// </summary>
    public class CameraControllerBasic : MonoBehaviour {


        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateAmount;


        private void Update() {
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

            Vector3 moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;
            transform.position += moveDir * moveSpeed * Time.deltaTime;


            float rotateAmount = 0f;
            if (Input.GetKey(KeyCode.Q)) {
                rotateAmount = +this.rotateAmount;
            }
            if (Input.GetKey(KeyCode.E)) {
                rotateAmount = -this.rotateAmount;
            }
            transform.eulerAngles += new Vector3(0, rotateAmount, 0) * Time.deltaTime;
        }
    }

}