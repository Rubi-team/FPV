using UnityEngine;
using UnityEngine.UI;
using System;

namespace CodeMonkey.Toolkit.TDrawMesh {

    public class DrawMeshUI : MonoBehaviour {


        [SerializeField] private Button thickness1Button;
        [SerializeField] private Button thickness2Button;
        [SerializeField] private Button thickness3Button;
        [SerializeField] private Button thickness4Button;
        [SerializeField] private Button color1Button;
        [SerializeField] private Button color2Button;
        [SerializeField] private Button color3Button;
        [SerializeField] private Button color4Button;


        private void Awake() {
            thickness1Button.onClick.AddListener(() => { SetThickness(0.2f); });
            thickness2Button.onClick.AddListener(() => { SetThickness(0.6f); });
            thickness3Button.onClick.AddListener(() => { SetThickness(1.0f); });
            thickness4Button.onClick.AddListener(() => { SetThickness(2.0f); });

            color1Button.onClick.AddListener(() => { SetColor(GetColorFromString("000000")); });
            color2Button.onClick.AddListener(() => { SetColor(GetColorFromString("FFFFFF")); });
            color3Button.onClick.AddListener(() => { SetColor(GetColorFromString("22FF00")); });
            color4Button.onClick.AddListener(() => { SetColor(GetColorFromString("0077FF")); });
        }

        private void SetThickness(float thickness) {
            DrawMesh.Instance.SetThickness(thickness);
        }

        private void SetColor(Color color) {
            DrawMesh.Instance.SetColor(color);
        }


        // Get Color from Hex string FF00FFAA
        private Color GetColorFromString(string color) {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8) {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }

        // Returns 0-255
        private int Hex_to_Dec(string hex) {
            return Convert.ToInt32(hex, 16);
        }

        private float Hex_to_Dec01(string hex) {
            return Hex_to_Dec(hex) / 255f;
        }

    }

}