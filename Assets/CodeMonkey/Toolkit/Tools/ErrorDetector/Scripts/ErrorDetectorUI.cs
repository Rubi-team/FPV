using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TErrorDetector {

    /// <summary>
    /// ** Error Detector **
    /// 
    /// When an error happens the ErrorDetector window will automatically show up.
    /// This is a great way to show to the Player of your game that something went
    /// wrong so they're not confused if weird things start to happen.
    /// 
    /// This window also keeps track of errors and will not show the exact same error twice,
    /// this is helpful when you have an error in your Update(); 
    /// that is firing all the time and you don't want the window to constantly keep showing up.
    /// 
    /// I recommend you combine this with Unity's Cloud Diagnostics which
    /// automatically captures errors from your game so you can then fix them.
    /// 
    /// Just Drag the Prefab onto your Canvas, no other setup required.
    /// </summary>
    public class ErrorDetectorUI : MonoBehaviour {


        [SerializeField] private TextMeshProUGUI errorTextMesh;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button copyToClipboardButton;


        private List<string> ignoreErrorStringList = new List<string>();


        private void Awake() {
            closeButton.onClick.AddListener(() => {
                Hide();
            });

            copyToClipboardButton.onClick.AddListener(() => {
                try {
                    GUIUtility.systemCopyBuffer = errorTextMesh.text;
                } catch (Exception e) {
                    Debug.LogException(new Exception("Failed to copy to Clipboard!\n" + e));
                }
            });
        }

        private void Start() {
            Application.logMessageReceived += Application_logMessageReceived;

            Hide();
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type) {
            if (type == LogType.Error || type == LogType.Exception) {
                errorTextMesh.text = "Error: " + condition + "\n" + stackTrace;

                if (ignoreErrorStringList.Contains(errorTextMesh.text)) {
                    // Error already shown
                    return;
                }

                ignoreErrorStringList.Add(errorTextMesh.text);

                Show();
            }
        }

        private void OnDestroy() {
            Application.logMessageReceived -= Application_logMessageReceived;
        }

        private void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

    }

}