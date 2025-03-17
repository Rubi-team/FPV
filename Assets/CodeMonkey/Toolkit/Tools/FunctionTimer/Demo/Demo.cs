using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TFunctionTimer.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button startTimer1Button;
        [SerializeField] private Button startTimer2Button;
        [SerializeField] private Button chainTimerButton;
        [SerializeField] private Button startCancelTimerButton;
        [SerializeField] private TextMeshProUGUI logTextMesh;


        private void Awake() {
            startTimer1Button.onClick.AddListener(() => {
                AddLog("Starting timer 0.3s...");
                FunctionTimer.Create(() => {
                    AddLog("Timer elapsed!");
                }, 0.3f);
            });

            startTimer2Button.onClick.AddListener(() => {
                AddLog("Starting timer 1s...");
                FunctionTimer.Create(() => {
                    AddLog("Timer elapsed!");
                }, 1f);
            });

            chainTimerButton.onClick.AddListener(() => {
                AddLog("Starting chained timers...");
                FunctionTimer.Create(() => {
                    AddLog("First one elapsed, starting another one");
                    FunctionTimer.Create(() => {
                        AddLog("Second one elapsed");
                    }, 0.7f);
                }, 0.3f);
            });

            startCancelTimerButton.onClick.AddListener(() => {
                AddLog("Starting timer...");
                FunctionTimer.Create(() => {
                    AddLog("Timer elapsed!");
                }, 1f, "Timer_1");
                AddLog("Cancelled Timer!");
                FunctionTimer.StopAllTimersWithName("Timer_1");
            });
        }

        private void AddLog(string logMessage) {
            logTextMesh.text = logMessage + "\n" + logTextMesh.text;
        }

    }

}