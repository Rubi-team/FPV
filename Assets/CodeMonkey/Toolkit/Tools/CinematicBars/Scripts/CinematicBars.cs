using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TCinematicBars {

    /// <summary>
    /// ** Cinematic Bars **
    /// 
    /// Black Cinematic Bars on top and bottom of the Canvas, great for cutscenes.
    /// 
    /// The class has auto-initialization, just call the static function CinematicBars.Show();
    /// Alternatively you can manually create a game object with this CinematicBars component inside your Canvas.
    /// That method is helpful if you want to easily handle sorting, like putting some Text dialog on top of the Cinematic Bars.
    /// </summary>
    public class CinematicBars : MonoBehaviour {


        private static CinematicBars instance;


        private RectTransform topBar;
        private RectTransform bottomBar;
        private float changeSizeAmount;
        private float targetSize;
        private bool isActive;


        private void Awake() {
            instance = this;

            GameObject gameObject = new GameObject("CinematicBars_TopBar", typeof(Image));
            gameObject.transform.SetParent(transform, false);
            gameObject.GetComponent<Image>().color = Color.black;
            topBar = gameObject.GetComponent<RectTransform>();
            topBar.anchorMin = new Vector2(0, 1);
            topBar.anchorMax = new Vector2(1, 1);
            topBar.sizeDelta = new Vector2(0, 0);
            topBar.pivot = new Vector2(0.5f, 1f);

            gameObject = new GameObject("CinematicBars_BottomBar", typeof(Image));
            gameObject.transform.SetParent(transform, false);
            gameObject.GetComponent<Image>().color = Color.black;
            bottomBar = gameObject.GetComponent<RectTransform>();
            bottomBar.anchorMin = new Vector2(0, 0);
            bottomBar.anchorMax = new Vector2(1, 0);
            bottomBar.sizeDelta = new Vector2(0, 0);
            bottomBar.pivot = new Vector2(0.5f, 0f);
        }

        private void Update() {
            if (isActive) {
                Vector2 sizeDelta = topBar.sizeDelta;
                sizeDelta.y += changeSizeAmount * Time.deltaTime;
                if (changeSizeAmount > 0) {
                    if (sizeDelta.y >= targetSize) {
                        sizeDelta.y = targetSize;
                        isActive = false;
                    }
                } else {
                    if (sizeDelta.y <= targetSize) {
                        sizeDelta.y = targetSize;
                        isActive = false;
                    }
                }
                topBar.sizeDelta = sizeDelta;
                bottomBar.sizeDelta = sizeDelta;
            }
        }

        private void ShowInstance(float targetSize, float time) {
            this.targetSize = targetSize;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
            isActive = true;
        }

        private void HideInstance(float time) {
            targetSize = 0f;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
            isActive = true;
        }


        private static void Init() {
            if (instance != null) {
                // Instance already created
                return;
            }
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null) {
                Debug.LogError("No Canvas was found in Scene! CinematicBars needs a Canvas to work.");
                return;
            }
            GameObject gameObject = new GameObject("CinematicBars", typeof(RectTransform));
            gameObject.transform.SetParent(canvas.transform);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            gameObject.AddComponent<CinematicBars>();
        }

        public static void Show(float targetSize, float time) {
            Init();
            if (instance != null) {
                instance.ShowInstance(targetSize, time);
            }
        }

        public static void Hide(float time) {
            Init();
            if (instance != null) {
                instance.HideInstance(time);
            }
        }

    }

}