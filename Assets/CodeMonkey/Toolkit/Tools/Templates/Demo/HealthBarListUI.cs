using UnityEngine;

namespace CodeMonkey.Toolkit.TTemplate.Demo {

    public class HealthBarListUI : MonoBehaviour {


        [SerializeField] private Transform template;
        [SerializeField] private Transform container;


        private void Awake() {
            template.gameObject.SetActive(false);
        }

        private void Start() {
            foreach (Transform child in container) {
                if (child == template) {
                    continue;
                }
                Destroy(child.gameObject);
            }

            for (int i = 0; i < 10; i++) {
                Transform singleTransform = Instantiate(template, container);
                singleTransform.gameObject.SetActive(true);
            }
        }

    }

}