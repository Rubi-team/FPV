using UnityEngine;

namespace CodeMonkey.Toolkit.TMousePosition {

    public class MousePositionPlane {


        public static Vector3 GetPosition() {
            Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(mouseCameraRay, out float distance)) {
                return mouseCameraRay.GetPoint(distance);
            } else {
                return Vector3.zero;
            }
        }

    }

}