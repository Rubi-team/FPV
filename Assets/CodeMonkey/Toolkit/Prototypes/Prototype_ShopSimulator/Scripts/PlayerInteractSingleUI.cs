using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class PlayerInteractSingleUI : MonoBehaviour {


        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textMesh;



        public void Setup(IInteractable.InteractAction interactAction, string text) {
            iconImage.sprite = GameAssetsShopSimulator.Instance.GetIconSprite(interactAction);
            textMesh.text = text;
        }

    }

}