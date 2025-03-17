using UnityEngine;
using TMPro;

namespace CodeMonkey.Toolkit.TFPSCounter {

    public class FPSCounter : MonoBehaviour {


        private TextMeshProUGUI textMesh;
        private int lastFrameIndex;
        private float[] frameDeltaTimeArray;


        private void Awake() {
            textMesh = GetComponent<TextMeshProUGUI>();
            int frameTotalAmount = 50;
            frameDeltaTimeArray = new float[frameTotalAmount];
        }

        private void Update() {
            textMesh.text = Mathf.RoundToInt(1f / Time.unscaledDeltaTime).ToString();

            frameDeltaTimeArray[lastFrameIndex] = Time.unscaledDeltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

            textMesh.text = Mathf.RoundToInt(CalculateFPS()).ToString();
        }

        private float CalculateFPS() {
            float total = 0f;
            foreach (float deltaTime in frameDeltaTimeArray) {
                total += deltaTime;
            }
            return frameDeltaTimeArray.Length / total;
        }

    }

}