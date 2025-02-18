using System;
using FMOD.Studio;
using FMODUnity;

[Serializable]
public class AudioModel
{
    // Name of the FMOD bank containing your audio event
    public string Bank;

    // Fully qualified FMOD event path (e.g., "event:/MyAudioEvent")
    public string EventName;
}