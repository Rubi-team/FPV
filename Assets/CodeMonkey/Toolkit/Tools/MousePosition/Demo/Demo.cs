using UnityEngine;

namespace CodeMonkey.Toolkit.TMousePosition.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Transform mousePositionPlaneTransform;
        [SerializeField] private Transform mousePosition2DTransform;
        [SerializeField] private Transform mousePositionRaycastTransform;
        


        private void Update() {
            mousePositionPlaneTransform.position = MousePositionPlane.GetPosition();
            mousePosition2DTransform.position = MousePosition2D.GetPosition();
            mousePositionRaycastTransform.position = MousePositionRaycast.GetPosition();
        }

    }

}