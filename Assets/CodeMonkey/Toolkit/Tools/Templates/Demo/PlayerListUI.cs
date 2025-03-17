using CodeMonkey.Toolkit.TFunctionPeriodic;
using CodeMonkey.Toolkit.TRandomData;
using UnityEngine;

namespace CodeMonkey.Toolkit.TTemplate.Demo {

    public class PlayerListUI : MonoBehaviour {


        [SerializeField] private Transform template;
        [SerializeField] private Transform container;
        [SerializeField] private Sprite[] spriteArray;


        private void Awake() {
            template.gameObject.SetActive(false);
        }

        private void Start() {
            FunctionPeriodic.Create(UpdateNameList, 2f);

            UpdateNameList();
        }

        private void UpdateNameList() {
            foreach (Transform child in container) {
                if (child == template) {
                    continue;
                }
                Destroy(child.gameObject);
            }

            for (int i = 0; i < Random.Range(2, 7); i++) {
                Transform singleTransform = Instantiate(template, container);
                singleTransform.gameObject.SetActive(true);
                Sprite sprite = spriteArray.GetRandomElement();
                string playerName = RandomData.GetRandomName(false);
                singleTransform.GetComponent<PlayerListSingleUI>().Setup(sprite, playerName);
            }
        }

    }

}