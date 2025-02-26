using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace Audio
{
    public static class AudioManager 
    {
        private static readonly Dictionary<int, EventInstance> _eventInstances = new();
        private static int _nextID;

        /// <summary>
        ///     Creates a new audio instance from the given AudioModel.
        /// </summary>
        public static void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            PlayOneShotRpc(sound.Path, worldPos);
        }

        [Rpc(SendTo.Everyone)] private static void PlayOneShotRpc(string soundPath, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(soundPath, worldPos);
        }
        
        
        public static void PlayOneShotAttached(EventReference sound, GameObject objectAttached)
        {
            PlayOneShotAttachedRpc(sound.Path, objectAttached.name);
        }
        
        [Rpc(SendTo.Everyone)] private static void PlayOneShotAttachedRpc(string soundPath, string objectName)
        {
            RuntimeManager.PlayOneShotAttached(soundPath, GameObject.Find(objectName));
        }

        /// <summary>
        ///     Creates a new audio instance from the given AudioModel.
        /// </summary>
        public static AudioInstance CreateAudioInstance(AudioModel model)
        {
            var instance = new AudioInstance { ID = _nextID++ };

            // Create an FMOD event instance using the event path from AudioModel.
            var eventInstance = RuntimeManager.CreateInstance(model.EventName);
            _eventInstances.Add(instance.ID, eventInstance);

            return instance;
        }

        /// <summary>
        ///     Tries to retrieve an FMOD event instance by its AudioInstance ID.
        /// </summary>
        public static bool TryGetEventInstance(int id, out EventInstance eventInstance)
        {
            return _eventInstances.TryGetValue(id, out eventInstance);
        }

        /// <summary>
        ///     Attaches the FMOD event instance to a GameObject.
        /// </summary>
        public static void AttachInstanceToGameObject(int id, Transform transform)
        {
            if (_eventInstances.TryGetValue(id, out var instance))
                RuntimeManager.AttachInstanceToGameObject(instance, transform);
        }
    }
}