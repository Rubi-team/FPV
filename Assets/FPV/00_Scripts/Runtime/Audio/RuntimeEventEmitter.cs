using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RuntimeEventEmitter : StudioEventEmitter
{
    public void SetEvent(EventReference newEventReference)
    {
        // Stop and release the current event instance if it exists
        if (EventInstance.isValid())
        {
            Stop();
            EventInstance.release();
        }

        // Create a new event instance
        EventInstance = RuntimeManager.CreateInstance(newEventReference);

        // Update the EventReference
        EventReference = newEventReference;

        // Attach to game object (maintains 3D positioning)
        RuntimeManager.AttachInstanceToGameObject(EventInstance, gameObject);
    }
}