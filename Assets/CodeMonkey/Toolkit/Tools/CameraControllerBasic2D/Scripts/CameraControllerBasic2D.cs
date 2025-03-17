using UnityEngine;

namespace CodeMonkey.Toolkit.TCameraControllerBasic2D {

    /// <summary>
    /// ** Camera Controller Basic 2D **
    /// 
    /// Very simple Camera Controller, great for quickly prototyping 2D games
    /// Just attach this script, then use WASD to move the camera.
    /// 
    /// Make sure this script is on a Transform that has the Camera as a Child object
    /// This Transform will move, which will also move the child Camera
    /// </summary>
    public class CameraControllerBasic2D : MonoBehaviour {


        [SerializeField] private float moveSpeed;


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

            Vector3 moveDir = transform.up * inputVector.y + transform.right * inputVector.x;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

    }

}