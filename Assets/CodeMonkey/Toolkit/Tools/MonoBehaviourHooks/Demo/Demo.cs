using TMPro;
using UnityEngine;

namespace CodeMonkey.Toolkit.TMonoBehaviourHooks.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private TextMeshProUGUI logTextMesh;


        private GameObject spawnedGameObject;


        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                if (spawnedGameObject != null) {
                    // Object already spawned
                    return;
                }
                spawnedGameObject = new GameObject();
                MonoBehaviourHooks monoBehaviourHooks = spawnedGameObject.AddComponent<MonoBehaviourHooks>();
                monoBehaviourHooks.onStartAction = () => {
                    logTextMesh.text = "Attached message on Start...\n" + logTextMesh.text;
                };
                monoBehaviourHooks.onEnableAction = () => {
                    logTextMesh.text = "Attached message on Enable...\n" + logTextMesh.text;
                };
                monoBehaviourHooks.onDisableAction = () => {
                    logTextMesh.text = "Attached message on Disable...\n" + logTextMesh.text;
                };
            }

            if (Input.GetKeyDown(KeyCode.Y)) {
                spawnedGameObject?.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.U)) {
                spawnedGameObject?.SetActive(true);
            }
        }

    }

}