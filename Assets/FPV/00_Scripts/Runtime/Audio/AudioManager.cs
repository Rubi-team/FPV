using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Audio
{
    public class AudioManager : NetworkBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        private static readonly Dictionary<int, EventInstance> _eventInstances = new();
        private static int _nextID;

        [SerializeField] private GameObject _emitterInstancePrefab;

        [field: Header("Player Movement")]
        [field: SerializeField] public EventReference footStep { get; private set; }

        [field: SerializeField] public EventReference walkConcreteFootStep { get; private set; }
        [field: SerializeField] public EventReference walkCarpetFootStep { get; private set; }
        [field: SerializeField] public EventReference walkWoodFootStep { get; private set; }
        [field: SerializeField] public EventReference walkMetalFootstep { get; private set; }
        
        [field: SerializeField] public EventReference runConcreteFootStep { get; private set; }
        [field: SerializeField] public EventReference runCarpetFootStep { get; private set; }
        [field: SerializeField] public EventReference runWoodFootStep { get; private set; }
        [field: SerializeField] public EventReference runMetalFootstep { get; private set; }
        
        [field: SerializeField] public EventReference jump { get; private set; }
        [field: SerializeField] public EventReference land { get; private set; }


        [field: Header("Player Action")]
        [field: SerializeField] public EventReference grabPlayer { get; private set; }
        [field: SerializeField] public EventReference grabItem { get; private set; }
        [field: SerializeField] public EventReference throwPlayer { get; private set; }
        [field: SerializeField] public EventReference throwItem { get; private set; }

        public EventReference takeDamage { get; private set; }


        [field: Header("Threat")]
        [field: SerializeField] public EventReference threatFootstep { get; private set; }
        [field: SerializeField] public  EventReference threatCharging { get; private set; }
        [field: SerializeField] public  EventReference threatHit { get; private set; }


        [field: Header("Objects")]
        [field: SerializeField] public EventReference furbyHit { get; private set; }
        [field: SerializeField] public EventReference furbyGrab { get; private set; }
        [field: SerializeField] public EventReference furbyFly { get; private set; }

        [field: SerializeField] public EventReference laserLoop { get; private set; }
        [field: SerializeField] public EventReference doorOpen { get; private set; }
        [field: SerializeField] public EventReference doorClose { get; private set; }
        [field: SerializeField] public EventReference target { get; private set; }

        /// <summary>
        ///     Plays a one-shot sound at the specified world position.
        /// </summary>
        public void PlayOneShot(EventReference sound, Vector3 worldPos, bool hasParameters = false,
            string parameter1Name = null, float parameter1Value = 0f, string parameter2Name = null,
            float parameter2Value = 0f)
        {
            if (hasParameters)
                PlayOneShotWithParametersRpc(sound.ToString(), worldPos, parameter1Name, parameter1Value,
                    parameter2Name, parameter2Value);
            else
                PlayOneShotRpc(sound.ToString(), worldPos);
        }

        [Rpc(SendTo.Everyone)]
        private void PlayOneShotRpc(string GUIDString, Vector3 worldPos)
        {
            var eventInstance = RuntimeManager.CreateInstance(StringToGUID(GUIDString));
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
            eventInstance.start();
            eventInstance.release();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayOneShotWithParametersRpc(string GUIDString, Vector3 worldPos, string parameter1Name,
            float parameter1Value, string parameter2Name, float parameter2Value)
        {
            var eventInstance = RuntimeManager.CreateInstance(StringToGUID(GUIDString));
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));

            if (!string.IsNullOrEmpty(parameter1Name))
                eventInstance.setParameterByName(parameter1Name, parameter1Value);

            if (!string.IsNullOrEmpty(parameter2Name))
                eventInstance.setParameterByName(parameter2Name, parameter2Value);

            eventInstance.start();
            eventInstance.release();
        }

        private static GUID StringToGUID(string eventName)
        {
            // Convert the string event name to a GUID
            return GUID.Parse(eventName);
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