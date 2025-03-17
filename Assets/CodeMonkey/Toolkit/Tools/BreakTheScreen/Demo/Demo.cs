using UnityEngine;

namespace CodeMonkey.Toolkit.TBreakTheScreen {

    public class Demo : MonoBehaviour {


        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                BreakTheScreen.Spawn();
            }
        }

    }

}