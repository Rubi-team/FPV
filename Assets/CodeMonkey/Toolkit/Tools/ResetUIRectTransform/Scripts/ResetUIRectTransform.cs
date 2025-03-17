using UnityEngine;

namespace CodeMonkey.Toolkit.TResetUIRectTransform {

    /// <summary>
    /// ** Reset UI RectTransform
    /// 
    /// Simple utility script to reset a UI position to 0,0 and full scale by default
    /// This is very useful for moving your UI windows around in the Editor
    /// But make them snap back to 0,0 when the game starts playing
    /// </summary>
    public class ResetUIRectTransform : MonoBehaviour {


        [SerializeField] private Vector2 anchoredPosition = Vector2.zero;
        [SerializeField] private Vector2 sizeDelta = Vector2.one;


        private void Awake() {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
        }

    }

}