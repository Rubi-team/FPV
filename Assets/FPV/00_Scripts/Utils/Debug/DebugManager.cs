using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class DebugManager : MonoBehaviour
    {
        public GameObject WebGLCanvas;
        public GameObject WarningPanel;

        private void Awake()
        {
#if DEBUG_ENABLED || UNITY_EDITOR
            WebGLCanvas.SetActive(true);
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene(0);

#if DEBUG_ENABLED || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H)) WarningPanel.SetActive(!WarningPanel.activeSelf);
#endif
        }
    }
}