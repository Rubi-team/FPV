using UnityEngine;

namespace CodeMonkey.Toolkit.TFPSCounter.Demo {

    public class WasteCPU : MonoBehaviour {


        private void Update() {
            float value = 3;
            for (int i = 0; i < 2000; i++) {
                value *= Random.Range(0, 100000);
                value = Mathf.Sqrt(value);
            }
        }

    }

}