using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShelfHeightWorldUI : MonoBehaviour {


        [SerializeField] private ShelfHeight shelfHeight;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private TextMeshPro textMesh;


        private void Start() {
            shelfHeight.OnObjectTypeChanged += ShelfHeight_OnObjectTypeChanged;
            PriceManager.Instance.OnPriceChanged += PriceManager_OnPriceChanged;

            Hide();
        }

        private void PriceManager_OnPriceChanged(object sender, System.EventArgs e) {
            UpdateVisual();
        }

        private void ShelfHeight_OnObjectTypeChanged(object sender, System.EventArgs e) {
            UpdateVisual();
        }

        private void UpdateVisual() {
            ObjectType objectType = shelfHeight.GetObjectType();
            if (objectType == ObjectType.None) {
                Hide();
            } else {
                Show(objectType, PriceManager.Instance.GetPrice(objectType));
            }
        }

        public void Show(ObjectType objectType, int price) {
            iconSpriteRenderer.enabled = true;
            GameAssetsShopSimulator.ObjectTypeBoxData objectTypeBoxData = GameAssetsShopSimulator.Instance.GetObjectTypeBoxData(objectType);
            iconSpriteRenderer.sprite = objectTypeBoxData.sprite;
            textMesh.text = GameAssetsShopSimulator.Instance.GetPriceString(price);
        }

        public void Hide() {
            iconSpriteRenderer.enabled = false;
            textMesh.text = "-";
        }

    }

}