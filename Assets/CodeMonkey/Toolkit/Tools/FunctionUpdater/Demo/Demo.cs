using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TFunctionUpdater.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button moveImageButton;
        [SerializeField] private Button chainMovingImageButton;
        [SerializeField] private Button startUpdaterButton;
        [SerializeField] private Button stopUpdaterButton;
        [SerializeField] private Button startInputListeningButton;
        [SerializeField] private Button stopInputListeningButton;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI logTextMesh;
        [SerializeField] private TextMeshProUGUI log2TextMesh;


        private void Awake() {
            moveImageButton.onClick.AddListener(() => {
                float timer = .5f;
                FunctionUpdater.Create(() => {
                    timer -= Time.deltaTime;
                    image.rectTransform.anchoredPosition += new Vector2(0, +100) * Time.deltaTime;
                    return timer <= 0f;
                }, "MoveImage", true, true);
            });

            chainMovingImageButton.onClick.AddListener(() => {
                float timer = .5f;
                FunctionUpdater.Create(() => {
                    timer -= Time.deltaTime;
                    image.rectTransform.anchoredPosition += new Vector2(0, +100) * Time.deltaTime;

                    bool functionDone = timer <= 0f;
                    if (functionDone) {
                        timer = .5f;
                        FunctionUpdater.Create(() => {
                            timer -= Time.deltaTime;
                            image.rectTransform.anchoredPosition += new Vector2(0, -100) * Time.deltaTime;
                            return timer <= 0f;
                        });
                        return true;
                    } else {
                        return false;
                    }
                }, "MoveImageBackForth", true, true);
            });

            startUpdaterButton.onClick.AddListener(() => {
                FunctionUpdater.Create(() => {
                    logTextMesh.text = "Update Frame: " + Time.frameCount + "\n" + logTextMesh.text;
                    return false;
                }, "LogUpdater", true, true);
            });

            stopUpdaterButton.onClick.AddListener(() => {
                logTextMesh.text = "Stopped Updater...\n" + logTextMesh.text;
                FunctionUpdater.StopAllUpdatersWithName("LogUpdater");
            });

            startInputListeningButton.onClick.AddListener(() => {
                log2TextMesh.text = "Press T or Y..." + "\n" + log2TextMesh.text;
                FunctionUpdater.Create(() => {
                    if (Input.GetKeyDown(KeyCode.T)) {
                        log2TextMesh.text = "Pressed T..." + "\n" + log2TextMesh.text;
                    }
                    if (Input.GetKeyDown(KeyCode.Y)) {
                        log2TextMesh.text = "Pressed Y..." + "\n" + log2TextMesh.text;
                    }
                    return false;
                }, "ListenInputs", true, true);
            });

            stopInputListeningButton.onClick.AddListener(() => {
                FunctionUpdater.StopAllUpdatersWithName("ListenInputs");
                log2TextMesh.text = "Stopped Listening\n" + log2TextMesh.text;
            });
        }
    }

}