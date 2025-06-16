using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject parentGObj;
    
    [SerializeField] private Slider masterVolumeSlider, chatVolumeSlider, sfxVolumeSlider, ambienceVolumeSlider, sensitivitySlider;
    private float sensitivity;
    private Bus masterBus, chatBus, sfxBus, ambienceBus;

    private void Awake()
    {
        SetupBus();
    }

    private void EnableUI()
    {
        parentGObj.SetActive(true);
        
        // Désactiver les inputs
        
    }

    private void DisableUI()
    {
        parentGObj.SetActive(false);
        
        // Ré-activer les inputs
        
    }

    public void ChangeVolume(int sliderIndex)
    {
        switch (sliderIndex)
        {
            case 0 : masterBus.setVolume(masterVolumeSlider.value);
                break;
            case 1 : chatBus.setVolume(chatVolumeSlider.value);
                break;
            case 2 : sfxBus.setVolume(sfxVolumeSlider.value);
                break;
            case 3 : ambienceBus.setVolume(ambienceVolumeSlider.value);
                break;
            case 4 : sensitivity = sensitivitySlider.value;
                break;
        }
    }

    private void SetupBus()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        chatBus = RuntimeManager.GetBus("bus:/Voice");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
    }
}
