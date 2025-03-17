using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TTemplate.Demo {

    public class PlayerListSingleUI : MonoBehaviour {


        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI textMesh;


        public void Setup(Sprite sprite, string name) {
            image.sprite = sprite;
            textMesh.text = name;
        }

    }

}