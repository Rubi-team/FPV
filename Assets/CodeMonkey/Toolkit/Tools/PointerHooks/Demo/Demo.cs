using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TPointerHooks.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI logTextMesh;


        private void Start() {
            PointerHooks pointerHooks = image.gameObject.AddComponent<PointerHooks>();
            pointerHooks.onPointerClickAction = (PointerEventData pointerEventData) => {
                logTextMesh.text = "Clicked!\n" + logTextMesh.text;
            };
            pointerHooks.onPointerEnterAction = (PointerEventData pointerEventData) => {
                logTextMesh.text = "On Enter!\n" + logTextMesh.text;
            };
            pointerHooks.onPointerExitAction = (PointerEventData pointerEventData) => {
                logTextMesh.text = "On Exit!\n" + logTextMesh.text;
            };
        }

    }

}