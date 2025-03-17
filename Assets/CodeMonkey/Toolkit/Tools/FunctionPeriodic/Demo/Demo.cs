using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TFunctionPeriodic.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button writeLogButton;
        [SerializeField] private Button spawnObjectButton;
        [SerializeField] private Button testKeyDownButton;
        [SerializeField] private TextMeshProUGUI logTextMesh;
        [SerializeField] private TextMeshProUGUI log2TextMesh;
        [SerializeField] private Transform prefab;
        [SerializeField] private Transform canvasTransform;


        private void Awake() {
            writeLogButton.onClick.AddListener(() => {
                FunctionPeriodic.Create(() => {
                    logTextMesh.text = "Tick!\n" + logTextMesh.text;
                }, 
                () => {
                    return false;
                },
                .5f, "Tick", true);
            });
            spawnObjectButton.onClick.AddListener(() => {
                FunctionPeriodic.Create(() => {
                    Transform spawnedTransform = Instantiate(prefab, canvasTransform);
                    spawnedTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-200, 200), Random.Range(-400, -100));
                },
                () => {
                    return false;
                },
                1f, "Spawn Object", true);
            });
            testKeyDownButton.onClick.AddListener(() => {
                FunctionPeriodic.Create(() => {
                    string text;
                    if (Input.GetKey(KeyCode.T)) {
                        text = "<color=#0f0>Key Down!</color>";
                    } else {
                        text = "<color=#ff0>Key Up!</color>";
                    }
                    log2TextMesh.text = text + "\n" + log2TextMesh.text;
                }, .5f);
            });
        }

    }

}