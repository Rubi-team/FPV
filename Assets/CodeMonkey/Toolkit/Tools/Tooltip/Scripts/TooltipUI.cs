using System;
using UnityEngine;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CodeMonkey.Toolkit.TTooltip {

    /// <summary>
    /// ** Tooltip **
    /// 
    /// Show a Tooltip to give the player some extra information
    /// It follows the mouse and has edge detection so it's always readable.
    /// 
    /// This class also has auto-initalization, 
    /// meaning you can either place it in the scene by default, 
    /// or when you call a function it will spawn the prefab from the Resources folder.
    /// 
    /// Just make sure the prefab is named exactly "TooltipUI" and is placed on a 
    /// folder named exactly "Resources", otherwise it won't work.
    /// </summary>
    public class TooltipUI : MonoBehaviour {


        private static TooltipUI instance;


        private static void Init() {
            if (instance == null) {
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas == null) {
                    Debug.LogError("No Canvas was found in Scene! " + nameof(TooltipUI) + " needs a Canvas to work.");
                    return;
                }
                TooltipUI tooltipCanvas = Resources.Load<TooltipUI>(nameof(TooltipUI));
                if (tooltipCanvas == null) {
                    Debug.LogError("Could not find " + nameof(TooltipUI) + " in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(TooltipUI) + "'?");
                    return;
                }
                instance = Instantiate(tooltipCanvas, canvas.transform);
            }
        }





        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private RectTransform backgroundRectTransform;
        [SerializeField] private Vector2 paddingSize = new Vector2(10, 10);
        [SerializeField] private Vector2 offset = new Vector2(5, 5);


        private Func<string> getTooltipStringFunc;
        private string lastTooltipText;
        private RectTransform rectTransform;


        private void Awake() {
            instance = this;

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

            Hide_Instance();
        }
        
        private void SetText(string tooltipText) {
            if (lastTooltipText == tooltipText) {
                // Same text
                return;
            }

            lastTooltipText = tooltipText;

            textMesh.SetText(tooltipText);
            textMesh.ForceMeshUpdate();

            Vector2 textSize = textMesh.GetRenderedValues(false);

            backgroundRectTransform.sizeDelta = textSize + paddingSize;
        }

        private void TestTooltip() {
            Show_Instance("Testing tooltip...");
        }

        private void Update() {
            if (getTooltipStringFunc == null || getTooltipStringFunc() == "") {
                return;
            }

            SetText(getTooltipStringFunc());
            UpdateVisual();
        }

        private void UpdateVisual() {
            Vector2 mousePosition = Input.mousePosition;
#if ENABLE_INPUT_SYSTEM
            mousePosition = Mouse.current.position.value;
#endif
            Vector2 anchoredPosition = (mousePosition / canvasRectTransform.localScale.x) + offset;

            float width = backgroundRectTransform.rect.width * rectTransform.localScale.x;
            if (anchoredPosition.x + width > canvasRectTransform.rect.width) {
                // Tooltip left screen on right side
                anchoredPosition.x = canvasRectTransform.rect.width - width;
            }
            float height = backgroundRectTransform.rect.height * rectTransform.localScale.y;
            if (anchoredPosition.y + height > canvasRectTransform.rect.height) {
                // Tooltip left screen on top side
                anchoredPosition.y = canvasRectTransform.rect.height - height;
            }

            rectTransform.anchoredPosition = anchoredPosition;
        }

        private void Show_Instance(string tooltipText) {
            Show_Instance(() => tooltipText);
        }

        private void Show_Instance(System.Func<string> getTooltipStringFunc) {
            this.getTooltipStringFunc = getTooltipStringFunc;
            gameObject.SetActive(true);
            SetText(getTooltipStringFunc());
            UpdateVisual();
        }

        private void Hide_Instance() {
            gameObject.SetActive(false);
        }





        public static void Show(string tooltipText) {
            Init();
            instance.Show_Instance(tooltipText);
        }

        public static void Show(Func<string> getTooltipStringFunc) {
            Init();
            instance.Show_Instance(getTooltipStringFunc);
        }

        public static void Hide() {
            Init();
            instance.Hide_Instance();
        }

    }

}