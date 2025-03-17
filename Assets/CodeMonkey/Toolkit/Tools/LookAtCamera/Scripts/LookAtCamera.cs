using UnityEngine;

namespace CodeMonkey.Toolkit.TLookAtCamera {

    /// <summary>
    /// ** LookAtCamera **
    /// 
    /// Simple script to make an object look at the camera
    /// Perfect for things like HealthBars where you want them to face the camera regardless of rotation
    /// 
    /// You can use the various methods to look at the camera in various ways
    /// </summary>
    public class LookAtCamera : MonoBehaviour {


        public enum Method {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted
        }


        [SerializeField] private Method method;


        private Transform mainCameraTransform;


        private void Awake() {
            mainCameraTransform = Camera.main.transform;
        }

        private void Update() {
            LookAt();
        }

        private void OnEnable() {
            LookAt();
        }

        public void SetMethod(Method method) {
            this.method = method;
        }

        private void LookAt() {
            switch (method) {
                default:
                case Method.LookAt:
                    transform.LookAt(mainCameraTransform.position);
                    break;
                case Method.LookAtInverted:
                    Vector3 dir = (transform.position - mainCameraTransform.position).normalized;
                    transform.LookAt(transform.position + dir);
                    break;
                case Method.CameraForward:
                    transform.forward = mainCameraTransform.forward;
                    break;
                case Method.CameraForwardInverted:
                    transform.forward = -mainCameraTransform.forward;
                    break;
            }
        }

    }

    public static class LookAtCameraExtensions {

        public static void AddLookAtCamera(this GameObject gameObject, LookAtCamera.Method method) {
            gameObject.AddComponent<LookAtCamera>().SetMethod(method);
        }

        public static void AddLookAtCamera(this Transform transform, LookAtCamera.Method method) {
            transform.gameObject.AddComponent<LookAtCamera>().SetMethod(method);
        }

    }

}