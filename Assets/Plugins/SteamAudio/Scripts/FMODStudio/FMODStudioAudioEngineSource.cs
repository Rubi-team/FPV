//
// Copyright 2017-2023 Valve Corporation.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#if STEAMAUDIO_ENABLED

using System;
using System.Reflection;
using Audio;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SteamAudio
{
    public sealed class FMODStudioAudioEngineSource : AudioEngineSource
    {
        private bool mFoundDSP = false;
        private StudioEventEmitter mEventEmitter = null;
        private EventInstance mEventInstance;
        private DSP mDSP;
        private SteamAudioSource mSteamAudioSource = null;
        private int mHandle = -1;

        private const int kSimulationOutputsParamIndex = 33;

        private int mRetryDSPFrames = 30;
        private int mCurrentRetryFrame = 0;

        public override void Initialize(GameObject gameObject)
        {
            FindDSP(gameObject);

            mSteamAudioSource = gameObject.GetComponent<SteamAudioSource>();
            if (mSteamAudioSource) mHandle = FMODStudioAPI.iplFMODAddSource(mSteamAudioSource.GetSource().Get());
        }

        public override void Destroy()
        {
            mFoundDSP = false;
            mCurrentRetryFrame = 0;

            if (mSteamAudioSource) FMODStudioAPI.iplFMODRemoveSource(mHandle);
        }

        public override void UpdateParameters(SteamAudioSource source)
        {
            CheckForChangedEventInstance();

            if (!mFoundDSP && mCurrentRetryFrame < mRetryDSPFrames)
            {
                FindDSP(source.gameObject);
                mCurrentRetryFrame++;
            }

            if (!mFoundDSP)
                return;

            mDSP.setParameterInt(kSimulationOutputsParamIndex, mHandle);
        }

        private void CheckForChangedEventInstance()
        {
            if (mEventEmitter != null)
            {
                var eventInstance = mEventEmitter.EventInstance;
                if (!eventInstance.Equals(mEventInstance))
                    mFoundDSP = false;
            }
            else
            {
                mFoundDSP = false;
            }
        }

        private void FindDSP(GameObject gameObject)
        {
            if (mFoundDSP)
                return;

            if (gameObject.TryGetComponent<VivoxToFmodConverter>(out var VivoxFmodConverter))
            {
                mEventInstance = VivoxFmodConverter._eventInstance;
                return;
            }

            mEventEmitter = gameObject.GetComponent<StudioEventEmitter>();
            if (mEventEmitter == null)
            {
                Debug.LogError($"FMODStudioAudioEngineSource: Aucun EventEmitter détecté sur {gameObject.name}");
                return;
            }

            try
            {
                mEventInstance = mEventEmitter.EventInstance;
            }
            catch (Exception ex)
            {
                Debug.LogError($"FMODStudioAudioEngineSource: Échec de récupération de l'EventInstance. Erreur : {ex.Message}");
                return;
            }

            if (!mEventInstance.isValid())
            {
                try
                {
                    var EventPath = mEventEmitter.EventReference.Path;

                    var audioModel = new AudioModel
                    {
                        EventName = EventPath
                    };

                    var AudioInstance = AudioManager.CreateAudioInstance(audioModel);

                    if (!AudioManager.TryGetEventInstance(AudioInstance.ID, out var eventInstance))
                    {
                        Debug.LogError("Failed to get event instance from AudioManager.");
                        return;
                    }

                    mEventInstance = eventInstance;
                    mEventEmitter.EventInstance = mEventInstance;
                }
                catch (Exception e)
                {
                    Debug.LogError($"FMODStudioAudioEngineSource: Exception lors de la création manuelle de l'event instance : {e}");
                    return;
                }

                if (!mEventInstance.isValid())
                {
                    Debug.LogError($"FMODStudioAudioEngineSource: L'instance d'événement n'est pas valide pour {gameObject.name}");
                    return;
                }
            }

            ChannelGroup channelGroup;
            var result = mEventInstance.getChannelGroup(out channelGroup);
            if (result != FMOD.RESULT.OK || !channelGroup.hasHandle())
                return;

            int numDSPs;
            result = channelGroup.getNumDSPs(out numDSPs);
            if (result != FMOD.RESULT.OK)
                return;

            for (var i = 0; i < numDSPs; ++i)
            {
                result = channelGroup.getDSP(i, out mDSP);
                if (result != FMOD.RESULT.OK)
                    continue;

                string dspName;
                uint version;
                int chans, configWidth, configHeight;
                mDSP.getInfo(out dspName, out version, out chans, out configWidth, out configHeight);

                if (dspName.Contains("Steam Audio") || dspName.Contains("phonon"))
                {
                    mFoundDSP = true;
                    Debug.Log($"[SteamAudio] DSP trouvé : {dspName} sur {gameObject.name}");
                    return;
                }
            }
        }
    }
}

#endif
