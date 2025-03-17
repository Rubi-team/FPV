using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TBlockerUI.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button testButton;
        [SerializeField] private TextMeshProUGUI textMesh;


        private int clickCounter = 0;


        private void Awake() {
            testButton.onClick.AddListener(() => {
                clickCounter++;
                textMesh.text = "Clicked! " + clickCounter + "\n" + textMesh.text;
            });
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                BlockerUI.Show();
            }
            if (Input.GetKeyDown(KeyCode.Y)) {
                BlockerUI.Hide();
            }
        }

    }

}
