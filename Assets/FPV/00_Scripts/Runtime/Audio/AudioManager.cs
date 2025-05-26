using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;

namespace Audio
{
    public class AudioManager : NetworkBehaviour
    {
        private static readonly Dictionary<int, EventInstance> _eventInstances = new();
        private static int _nextID;

        [SerializeField] private GameObject _emitterInstancePrefab;
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        ///     Creates a new audio instance from the given AudioModel.
        /// </summary>
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            PlayOneShotRpc(sound.ToString(), worldPos);
        }

        [Rpc(SendTo.Everyone)]
        private void PlayOneShotRpc(string soundPath, Vector3 worldPos)
        {
            var emitterInstance = Instantiate(_emitterInstancePrefab, worldPos, Quaternion.identity);
            var emitter = emitterInstance.GetComponent<StudioEventEmitter>();

            var eventReference = RuntimeManager.PathToGUID(soundPath);
            emitter.EventReference.Guid = eventReference;

            emitter.Play();

            // Destroy the emitter instance after the sound has played
            Destroy(emitterInstance, 1f);
        }


        public void PlayOneShotAttached(EventReference sound, GameObject objectAttached)
        {
            PlayOneShotAttachedRpc(sound.ToString(), objectAttached.name);
        }

        [Rpc(SendTo.Everyone)]
        private void PlayOneShotAttachedRpc(string soundPath, string objectName)
        {
            RuntimeManager.PlayOneShotAttached(soundPath, GameObject.Find(objectName));
        }

        /// <summary>
        ///     Creates a new audio instance from the given AudioModel.
        /// </summary>
        public static AudioInstance CreateAudioInstance(AudioModel eventReference)
        {
            var instance = new AudioInstance { ID = _nextID++ };

            // Create an FMOD event instance using the event Reference from AudioModel.
            var eventInstance = RuntimeManager.CreateInstance(eventReference.EventName);
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
                RuntimeManager.AttachInstanceToGameObject(instance, transform.gameObject);
        }
    }
}