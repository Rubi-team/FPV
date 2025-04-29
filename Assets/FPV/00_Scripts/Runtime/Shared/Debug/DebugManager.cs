using System;
using FPV.Runtime.Shared;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Unity.Netcode; // Assure-toi que cette ligne est présente

namespace FPV
{
    public class DebugManager : MonoBehaviour
    {
        public GameObject WebGLCanvas;
        public GameObject WarningPanel;

        [SerializeField] private TMP_Text versionText;
        [SerializeField] private TMP_Text relayCodeText;

#if UNITY_EDITOR
        [SerializeField] private NetworkManager networkManagerPrefab;
        private NetworkManager spawnedNetworkManager;
#endif

        private void Awake()
        {
#if DEBUG || UNITY_EDITOR
            WebGLCanvas.SetActive(true);
#endif
            versionText.text = Application.version;

            Debug.Log("UI");
        }

        private void Update()
        {
            relayCodeText.text = "Relay Code: " + RelayManager.JoinCode;

#if DEBUG || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H)) WarningPanel.SetActive(!WarningPanel.activeSelf);
            if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene(0);

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (spawnedNetworkManager == null)
                {
                    spawnedNetworkManager = Instantiate(networkManagerPrefab);

                    spawnedNetworkManager.gameObject.GetComponent<CustomNetworkManager>().SinglePlayerMode();

                    spawnedNetworkManager.StartHost();

                    Destroy(spawnedNetworkManager.gameObject.GetComponent<CustomNetworkManager>());

                    Debug.Log("pine ta mere Eliot j'espere ça marche");
                }
                else
                {
                    Debug.LogWarning("Appuye pas 2 fois bouffon");
                }
            }
#endif

#endif
        }
    }
}