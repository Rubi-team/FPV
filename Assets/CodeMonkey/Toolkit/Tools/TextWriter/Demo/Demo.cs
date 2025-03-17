using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TTextWriter.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private TextMesh textMesh;
        [SerializeField] private Text uiText;
        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Button writeTextMeshButton;
        [SerializeField] private Button writeUITextButton;
        [SerializeField] private Button writeTextMeshProButton;
        [SerializeField] private Button writeTextMeshProUGUIButton;
        [SerializeField] private TextMeshPro textMeshProTesting;
        [SerializeField] private Button writeFastButton;
        [SerializeField] private Button writeSlowButton;
        [SerializeField] private Button writeWithInvisibleButton;
        [SerializeField] private Button writeWithoutInvisibleButton;
        [SerializeField] private Button writeCallbacksButton;


        private void Start() {
            string textToWrite = "Hello and Welcome, I'm your Code Monkey!";
            writeTextMeshButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMesh, "TextMesh: " + textToWrite, .05f, true, false, null);
            });
            writeUITextButton.onClick.AddListener(() => {
                TextWriter.AddWriter(uiText, "Text (UI): " + textToWrite, .05f, true, false, null);
            });
            writeTextMeshProButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshPro, "TextMeshPro: " + textToWrite, .05f, true, false, null);
            });
            writeTextMeshProUGUIButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProUGUI, "TextMeshProUGUI: " + textToWrite, .05f, true, false, null);
            });

            writeFastButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProTesting, textToWrite, .01f, true, false, null);
            });
            writeSlowButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProTesting, textToWrite, .1f, true, false, null);
            });
            writeWithInvisibleButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProTesting, textToWrite, .05f, true, false, null);
            });
            writeWithoutInvisibleButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProTesting, textToWrite, .05f, false, false, null);
            });
            writeCallbacksButton.onClick.AddListener(() => {
                TextWriter.AddWriter(textMeshProTesting, "This is using the OnComplete callback...", .05f, true, false, () => {
                    TextWriter.AddWriter(textMeshProTesting, "to write text after text...", .05f, true, false, () => {
                        TextWriter.AddWriter(textMeshProTesting, "after more text...", .05f, true, false, () => {
                            TextWriter.AddWriter(textMeshProTesting, "or you can trigger anything you want after the text is written.", .05f, true, false, () => {
                            });
                        });
                    });
                });
            });
        }

    }

}