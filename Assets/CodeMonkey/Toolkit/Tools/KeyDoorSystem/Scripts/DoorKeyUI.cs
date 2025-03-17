using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TKeyDoorSystem {

    public class DoorKeyUI : MonoBehaviour {


        [Tooltip("The Prefab for a single UI Key")]
        public Transform keyUISinglePrefab;

        [Tooltip("The Key Holder that will be displayed")]
        public DoorKeyHolder doorKeyHolder;


        private void Start() {
            if (doorKeyHolder == null) {
                Debug.LogError("You need to set the DoorKeyHolder field on the DoorKeyUI! Drag the Player Game Object onto it.");
                return;
            }
            doorKeyHolder.OnDoorKeyAdded += DoorKeyHolder_OnDoorKeyAdded;
            doorKeyHolder.OnDoorKeyUsed += DoorKeyHolder_OnDoorKeyUsed;

            RefreshKeyVisuals();
        }

        private void DoorKeyHolder_OnDoorKeyUsed(object sender, System.EventArgs e) {
            RefreshKeyVisuals();
        }

        private void DoorKeyHolder_OnDoorKeyAdded(object sender, System.EventArgs e) {
            RefreshKeyVisuals();
        }

        private void RefreshKeyVisuals() {
            // Destroy old Key visuals
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            // Refresh current Key visuals
            int totalKeyCount = doorKeyHolder.GetDoorKeyHoldingList().Count;
            float keyPositionDistance = 50f;
            float keyPositionOffset = (Mathf.Max(totalKeyCount, 1) - 1) / 2f * keyPositionDistance;
            int index = 0;
            foreach (Key key in doorKeyHolder.GetDoorKeyHoldingList()) {
                // Instantiate UI Prefab
                Transform keyUISingleTransform = Instantiate(keyUISinglePrefab, transform);
                // Set UI Key Color
                keyUISingleTransform.GetComponent<Image>().color = key.keyColor;
                // Position UI Key
                RectTransform keyRectTransform = keyUISingleTransform.GetComponent<RectTransform>();
                keyRectTransform.anchoredPosition = new Vector2(index * keyPositionDistance - keyPositionOffset, 0);

                index++;
            }
        }

    }

}