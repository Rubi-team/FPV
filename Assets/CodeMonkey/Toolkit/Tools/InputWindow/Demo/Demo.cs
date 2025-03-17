using CodeMonkey.Toolkit.TBlockerUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CodeMonkey.Toolkit.TInputWindow.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button askPlayerNameButton;
        [SerializeField] private TextMeshProUGUI askPlayerNameTextMesh;
        [SerializeField] private Button askAgeButton;
        [SerializeField] private TextMeshProUGUI askAgeTextMesh;


        private void Awake() {
            askPlayerNameButton.onClick.AddListener(() => {
                BlockerUI.Show();
                InputWindowUI.Show(
                    "Player Name", 
                    "Code Monkey", 
                    InputWindowUI.ALPHABET_LOWER_CASE + InputWindowUI.ALPHABET_UPPER_CASE + InputWindowUI.NUMBERS + " _-", 
                    30, 
                    () => {
                        BlockerUI.Hide();
                        askPlayerNameTextMesh.text = "Ask Player Name\nCancelled!";
                    }, 
                    (string chosenName) => {
                        BlockerUI.Hide();
                        askPlayerNameTextMesh.text = "Ask Player Name\n Name: " + chosenName;
                    }
                );
            });

            askAgeButton.onClick.AddListener(() => {
                BlockerUI.Show();
                InputWindowUI.Show(
                    "Age (numbers only)",
                    0,
                    () => {
                        BlockerUI.Hide();
                        askAgeTextMesh.text = "Ask Age (numbers only)\nCancelled!";
                    },
                    (int age) => {
                        BlockerUI.Hide();
                        askAgeTextMesh.text = "Ask Age (numbers only)\n Age: " + age;
                    }
                );
            });
        }

    }

}