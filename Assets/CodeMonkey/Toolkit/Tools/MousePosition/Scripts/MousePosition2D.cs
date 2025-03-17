using UnityEngine;

namespace CodeMonkey.Toolkit.TMousePosition {

    public class MousePosition2D {


        public static Vector3 GetPosition() {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0f;
            return position;
        }


    }

}