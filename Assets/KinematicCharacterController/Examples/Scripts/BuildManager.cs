using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject WebGLCanvas;
    public GameObject WarningPanel;

    void Awake()
    {
#if ENABLE_FEATURE_X
        WebGLCanvas.SetActive(true);
#endif
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

#if ENABLE_FEATURE_X
        if(Input.GetKeyDown(KeyCode.H))
        {
            WarningPanel.SetActive(!WarningPanel.activeSelf);
        }
#endif
    }
}
