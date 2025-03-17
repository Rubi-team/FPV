using CodeMonkey.Toolkit.TFunctionPeriodic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TTextPopup {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button spawnTextPopupUIButton;
        [SerializeField] private Button spawnUpdatingTextPopupUIButton;
        [SerializeField] private Button spawnTextPopupWorldButton;
        [SerializeField] private Button spawnUpdatingTextPopupWorldButton;


        private string textString;


        private void Awake() {
            textString = "Testing...";

            spawnTextPopupUIButton.onClick.AddListener(() => {
                TextPopupUI.Create(spawnTextPopupUIButton.GetComponent<RectTransform>().anchoredPosition, "Clicked!", 2f);
            });
            spawnUpdatingTextPopupUIButton.onClick.AddListener(() => {
                TextPopupUI.Create(spawnUpdatingTextPopupUIButton.GetComponent<RectTransform>().anchoredPosition, () => textString, 2f);
            });
            spawnTextPopupWorldButton.onClick.AddListener(() => {
                TextPopupWorld.Create(new Vector3(49.8f, 4.35f, 25.7f), "Clicked! (world)", .1f);
            });
            spawnUpdatingTextPopupWorldButton.onClick.AddListener(() => {
                TextPopupWorld.Create(new Vector3(46.5f, 4.8f, 28f), () => textString, .08f);
            });

            // Modify textString periodically, then update the TextPopup
            FunctionPeriodic.Create(() => {
                string abc = "qwertyuiopasdfghjklOQWEERTYSDVPIOSDFNMLM\n\n\n";
                string text = "Hello and Welcome, I'm your Code Monkey!\nRandom text:\n";
                for (int i = 0; i < Random.Range(5, 200); i++) {
                    text += abc[Random.Range(0, abc.Length)];
                }
                textString = text;
            }, .5f);
        }

    }

}