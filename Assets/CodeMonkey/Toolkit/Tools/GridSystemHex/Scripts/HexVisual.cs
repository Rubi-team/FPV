using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.TGridSystemHex {

    public class HexVisual : MonoBehaviour {


        [SerializeField] private Transform selectedTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material redMaterial;
        [SerializeField] private TextMeshPro textMesh;


        private void Start() {
            HideSelected();
        }

        public void SetText(string text) {
            textMesh.text = text;
        }

        public void SetRedColor() {
            meshRenderer.material = redMaterial;
        }

        public void ShowSelected() {
            selectedTransform.gameObject.SetActive(true);
        }

        public void HideSelected() {
            selectedTransform.gameObject.SetActive(false);
        }

    }

}