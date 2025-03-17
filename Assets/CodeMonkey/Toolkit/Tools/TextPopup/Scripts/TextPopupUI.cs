using System;
using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.TTextPopup {

    public class TextPopupUI : MonoBehaviour {


        public static Vector2 ConvertPositionPivotCenterToLowerLeftCorner(Vector2 anchoredPosition, float canvasRectTransformScale) {
            return
                new Vector2(Screen.width, Screen.height) * .5f / canvasRectTransformScale + anchoredPosition;
        }

        public static TextPopupUI Create(Vector2 anchoredPosition, string text, float scale = 2f, float destroyTimer = 3f, bool convertPositionPivotCenterToLowerLeftCorner = true) {
            return Create(anchoredPosition, () => text, scale, destroyTimer, convertPositionPivotCenterToLowerLeftCorner);
        }

        public static TextPopupUI Create(Vector2 anchoredPosition, Func<string> getTextStringFunc, float scale = 2f, float destroyTimer = 3f, bool convertPositionPivotCenterToLowerLeftCorner = true) {
            TextPopupUI textPopupUIPrefab = Resources.Load<TextPopupUI>(nameof(TextPopupUI));
            if (textPopupUIPrefab == null) {
                Debug.LogError("Could not find " + nameof(TextPopupUI) + " in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(TextPopupUI) + "'?");
                return null;
            }

            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null) {
                Debug.LogError("No Canvas was found in Scene! " + nameof(TextPopupUI) + " needs a Canvas to work.");
                return null;
            }

            if (convertPositionPivotCenterToLowerLeftCorner) {
                anchoredPosition = ConvertPositionPivotCenterToLowerLeftCorner(anchoredPosition, canvas.transform.localScale.x);
            }

            TextPopupUI textPopupUI = Instantiate(textPopupUIPrefab, canvas.transform);
            textPopupUI.transform.localScale = Vector3.one * scale;

            textPopupUI.Setup(anchoredPosition, getTextStringFunc);

            Destroy(textPopupUI.gameObject, destroyTimer);

            return textPopupUI;
        }



        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private RectTransform backgroundRectTransform;
        [SerializeField] private Vector2 paddingSize = new Vector2(10, 10);


        private Func<string> getTextStringFunc;
        private string lastText;
        private RectTransform canvasRectTransform;
        private RectTransform rectTransform;
        private Vector2 startingAnchoredPosition;


        private void Awake() {
            rectTransform = GetComponent<RectTransform>();

            if (canvasRectTransform == null) {
                Canvas canvas = transform.GetComponentInParent<Canvas>();
                if (canvas != null) {
                    canvasRectTransform = canvas.GetComponent<RectTransform>();
                }
            }

            if (canvasRectTransform == null) {
                Debug.LogError("Need to set Canvas Rect Transform!");
            }

            textMesh.GetComponent<RectTransform>().anchoredPosition = paddingSize * .5f;
        }

        private void Update() {
            SetText(getTextStringFunc());
            UpdateVisual();
        }

        private void Setup(Vector2 startingAnchoredPosition, Func<string> getTextStringFunc) {
            this.startingAnchoredPosition = startingAnchoredPosition;
            this.getTextStringFunc = getTextStringFunc;

            SetText(getTextStringFunc());
            UpdateVisual();
        }

        private void SetText(string text) {
            if (lastText == text) {
                // Same text
                return;
            }

            textMesh.SetText(text);
            textMesh.ForceMeshUpdate();
            Vector2 textSize = textMesh.GetRenderedValues(false);

            backgroundRectTransform.sizeDelta = textSize + paddingSize;
        }

        private void UpdateVisual() {
            Vector2 anchoredPosition = startingAnchoredPosition;

            float width = backgroundRectTransform.rect.width * rectTransform.localScale.x;
            if (anchoredPosition.x + width > canvasRectTransform.rect.width) {
                // Popup left screen on right side
                anchoredPosition.x = canvasRectTransform.rect.width - width;
            }
            float height = backgroundRectTransform.rect.height * rectTransform.localScale.y;
            if (anchoredPosition.y + height > canvasRectTransform.rect.height) {
                // Popup left screen on top side
                anchoredPosition.y = canvasRectTransform.rect.height - height;
            }
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public void DestroySelf() {
            if (this != null && gameObject != null) {
                Destroy(gameObject);
            }
        }

    }

}