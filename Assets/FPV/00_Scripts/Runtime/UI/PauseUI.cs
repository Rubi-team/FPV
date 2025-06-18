using System;
using FMOD.Studio;
using FMODUnity;
using FPV;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject parentGObj;
    
    [SerializeField] private Slider masterVolumeSlider, chatVolumeSlider, sfxVolumeSlider, ambienceVolumeSlider, sensitivitySlider;
    public float sensitivity;
    private Bus masterBus, chatBus, sfxBus, ambienceBus;
    public bool pauseMenuActive;
    
    public static PauseUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        SetupBus();
    }

    private void Start()
    {
        ChangeVolume(0); // Initialize master volume
        ChangeVolume(1); // Initialize chat volume
        ChangeVolume(2); // Initialize SFX volume
        ChangeVolume(3); // Initialize ambience volume
        ChangeVolume(4); // Initialize sensitivity
    }

    public void ChangeUI(bool state)
    {
        parentGObj.SetActive(state);
        pauseMenuActive = state;
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
