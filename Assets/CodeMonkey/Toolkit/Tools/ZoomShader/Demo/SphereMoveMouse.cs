using UnityEngine;
using CodeMonkey.Toolkit.TMousePosition;

namespace CodeMonkey.Toolkit.TZoomShader {

    /// <summary>
    /// Simple script to make this Sphere object follow the mouse position
    /// </summary>
    public class SphereMoveMouse : MonoBehaviour {


        private void Update() {
            transform.position = MousePositionRaycast.GetPosition();
        }

    }

}