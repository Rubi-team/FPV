using UnityEngine;

namespace CodeMonkey.Toolkit.TZoomShader {

    /// <summary>
    /// Sets the position on the shader to make it zoom in properly
    /// </summary>
    public class ZoomShaderScreenPosition : MonoBehaviour {


        [SerializeField] private Material material;


        private void Update() {
            Vector2 screenPixels = Camera.main.WorldToScreenPoint(transform.position);
            screenPixels = new Vector2(screenPixels.x / Screen.width, screenPixels.y / Screen.height);

            material.SetVector("_ObjectScreenPosition", screenPixels);
        }

    }

}