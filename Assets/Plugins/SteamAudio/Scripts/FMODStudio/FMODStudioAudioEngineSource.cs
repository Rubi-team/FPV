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
using UnityEngine;

namespace SteamAudio
{
    public sealed class FMODStudioAudioEngineSource : AudioEngineSource
    {
        private bool mFoundDSP = false;
        private FMODUnity.StudioEventEmitter mEventEmitter = null;
        private FMOD.Studio.EventInstance mEventInstance;
        private FMOD.DSP mDSP;
        private SteamAudioSource mSteamAudioSource = null;
        private int mHandle = -1;

        private const int kSimulationOutputsParamIndex = 33;

        public override void Initialize(GameObject gameObject)
        {
            FindDSP(gameObject);

            mSteamAudioSource = gameObject.GetComponent<SteamAudioSource>();
            if (mSteamAudioSource) mHandle = FMODStudioAPI.iplFMODAddSource(mSteamAudioSource.GetSource().Get());
        }

        public override void Destroy()
        {
            mFoundDSP = false;

            if (mSteamAudioSource) FMODStudioAPI.iplFMODRemoveSource(mHandle);
        }

        public override void UpdateParameters(SteamAudioSource source)
        {
            CheckForChangedEventInstance();

            FindDSP(source.gameObject);
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
                    // The event instance is different from the one we last used, which most likely means the
                    // event-related objects were destroyed and re-created. Make sure we look for the DSP instance
                    // when FindDSP is called next.
                    mFoundDSP = false;
            }
            else
            {
                // We haven't yet seen a valid event emitter component, so make sure we look for one when
                // FindDSP is called.
                mFoundDSP = false;
            }
        }

        private void FindDSP(GameObject gameObject)
        {
            if (mFoundDSP)
                return;


            try
            {
                mEventInstance = gameObject.GetComponent<VivoxToFmodConverter>()._eventInstance;
                if (!mEventInstance.isValid())
                {
                    Debug.LogError("Vivox Fmod Event instance is not valid");
                    throw new NullReferenceException();
                }
            }
            catch (Exception)
            {
                mEventEmitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
                mEventInstance = mEventEmitter.EventInstance;
                return;
            }

            if (!mEventInstance.isValid())
                return;

            FMOD.ChannelGroup channelGroup;
            mEventInstance.getChannelGroup(out channelGroup);

            int numDSPs;
            channelGroup.getNumDSPs(out numDSPs);

            for (var i = 0; i < numDSPs; ++i)
            {
                channelGroup.getDSP(i, out mDSP);

                var dspName = "";
                var dspVersion = 0u;
                var dspNumChannels = 0;
                var dspConfigWidth = 0;
                var dspConfigHeight = 0;
                mDSP.getInfo(out dspName, out dspVersion, out dspNumChannels, out dspConfigWidth, out dspConfigHeight);

                if (dspName == "Steam Audio Spatializer")
                {
                    mFoundDSP = true;
                    return;
                }
            }
        }
    }
}

#endif