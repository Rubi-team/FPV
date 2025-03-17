using UnityEngine;

namespace CodeMonkey.Toolkit.TLookAtCamera.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Transform cameraManagerTransform;


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

            Vector3 moveDir = cameraManagerTransform.forward * inputVector.y + cameraManagerTransform.right * inputVector.x;
            float moveSpeed = 10f;
            cameraManagerTransform.position += moveDir * moveSpeed * Time.deltaTime;


            float rotateAmount = 0f;
            if (Input.GetKey(KeyCode.Q)) {
                rotateAmount = +90;
            }
            if (Input.GetKey(KeyCode.E)) {
                rotateAmount = -90;
            }
            cameraManagerTransform.eulerAngles += new Vector3(0, rotateAmount, 0) * Time.deltaTime;
        }

    }

}