using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeMonkey.Toolkit.TTooltip {

    /// <summary>
    /// Show Tooltip on mouse/pointer over/out on this object
    /// You can either pre-set the tooltipMessage, or dynamically call SetTooltipMessage();
    /// </summary>
    public class TooltipOverOut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {


        [SerializeField] private string tooltipMessage;


        private bool isVisible;



        public void OnPointerEnter(PointerEventData eventData) {
            TooltipUI.Show(tooltipMessage);
            isVisible = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            TooltipUI.Hide();
            isVisible = false;
        }

        public void SetTooltipMessage(string tooltipMessage) {
            this.tooltipMessage = tooltipMessage;
            if (isVisible) {
                TooltipUI.Show(tooltipMessage);
            }
        }

        private void OnDisable() {
            if (isVisible) {
                TooltipUI.Hide();
            }
        }

    }

}