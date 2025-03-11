using System;
using FPV;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPV
{
    public class DebugManager : MonoBehaviour
    {
        public GameObject WebGLCanvas;
        public GameObject WarningPanel;
        
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private TMP_Text relayCodeText;

        private void Awake()
        {
#if DEBUG_ENABLED || UNITY_EDITOR
            WebGLCanvas.SetActive(true);
#endif
            versionText.text = Application.version;
            
            Console.Log("UI", versionText.text);
        }

        private void Update()
        {
            relayCodeText.text = "Relay Code: " + RelayManager.JoinCode;

#if DEBUG_ENABLED || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H)) WarningPanel.SetActive(!WarningPanel.activeSelf);
            if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene(0);
#endif
        }
    }
}