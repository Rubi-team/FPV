using CodeMonkey.Toolkit.TLookAtCamera;
using System;
using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.TTextPopup {

    public class TextPopupWorld : MonoBehaviour {


        public static TextPopupWorld Create(Vector3 worldPosition, string text, float scale = .1f, float destroyTimer = 3f) {
            return Create(worldPosition, () => text, scale, destroyTimer);
        }

        public static TextPopupWorld Create(Vector3 worldPosition, Func<string> getTextStringFunc, float scale = .1f, float destroyTimer = 3f) {
            TextPopupWorld textPopupWorldPrefab = Resources.Load<TextPopupWorld>(nameof(TextPopupWorld));
            if (textPopupWorldPrefab == null) {
                Debug.LogError("Could not find " + nameof(TextPopupWorld) + " in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(TextPopupWorld) + "'?");
                return null;
            }

            TextPopupWorld textPopupWorld = Instantiate(textPopupWorldPrefab, worldPosition, Quaternion.identity);
            textPopupWorld.transform.localScale = Vector3.one * scale;
            textPopupWorld.gameObject.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

            textPopupWorld.Setup(getTextStringFunc);

            Destroy(textPopupWorld.gameObject, destroyTimer);

            return textPopupWorld;
        }



        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private Transform backgroundCube;
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private Vector2 paddingSize = new Vector2(10, 10);


        private Func<string> getTextStringFunc;
        private string lastText;
        private RectTransform rectTransform;
        private Vector2 startingAnchoredPosition;


        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update() {
            SetText(getTextStringFunc());
        }

        private void Setup(Func<string> getTextStringFunc) {
            this.getTextStringFunc = getTextStringFunc;

            SetText(getTextStringFunc());
        }

        private void SetText(string text) {
            if (lastText == text) {
                // Same text
                return;
            }

            textMesh.SetText(text);
            textMesh.ForceMeshUpdate();
            Vector2 textSize = textMesh.GetRenderedValues(false);

            backgroundSpriteRenderer.size = textSize + paddingSize;
            Vector3 backgroundCubeScale = textSize + paddingSize * .5f;
            backgroundCubeScale.z = .1f;
            backgroundCube.localScale = backgroundCubeScale;

            Vector3 offset = new Vector3(-2f, 0f);
            backgroundSpriteRenderer.transform.localPosition =
                new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;
            backgroundCube.localPosition =
                new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f, +.1f) + offset;
        }

        public void DestroySelf() {
            if (this != null && gameObject != null) {
                Destroy(gameObject);
            }
        }

    }

}