using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Audio;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FPV
{
    public class VivoxToFmodConverter : MonoBehaviour
    {
        private const int LatencyMS = 50;
        private const int DriftMS = 1;
        private const float DriftCorrectionPercentage = 0.5f;

        private readonly List<float> audioBuffer = new();
        private int actualLatency;
        private uint adjustedLatency;
        private EVENT_CALLBACK audioCallback;

        private AudioModel audioModel;
        private uint bufferReadPosition;
        private uint bufferSamplesWritten;
        private Channel channel;
        private DSP compressorDSP;
        private uint driftThreshold;
        private EventInstance eventInstance;

        private bool isSpeaking;
        private uint minimumSamplesWritten = uint.MaxValue;
        private Sound sound;

        private CREATESOUNDEXINFO soundInfo;

        private int systemSampleRate;
        private uint targetLatency;
        private uint totalSamplesRead;
        private uint totalSamplesWritten;

        private AudioInstance AudioInstance { set; get; }

        private void OnDestroy()
        {
            sound.release();
            compressorDSP.release();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!channel.hasHandle()) return;

            audioBuffer.AddRange(data);
            UpdateBufferLatency((uint)data.Length);

            // On vérifie si le compresseur est attaché, sinon on l'ajoute
            var isDspAdded = false;
            channel.getDSP(0, out var firstDSP);
            if (firstDSP.hasHandle())
            {
                DSP_TYPE dspType;
                firstDSP.getType(out dspType);
                isDspAdded = dspType == DSP_TYPE.COMPRESSOR;
            }

            if (!isDspAdded) channel.addDSP(0, compressorDSP);
            // Ajout Compresseur Audio
            isSpeaking = Array.Exists(data, value => value != 0);
            ProcessAudio(channels);

            // On efface les données d'entrée pour éviter les effets de feedback
            for (var i = 0; i < data.Length; i++)
                data[i] = 0;
        }

        public void Setup(AudioModel audioModelSetup)
        {
            audioModel = audioModelSetup;
            systemSampleRate = AudioSettings.outputSampleRate;

            if (!AudioBankLoader.HasBankLoaded(audioModel.Bank))
                AudioBankLoader.LoadBank(audioModel.Bank, true, CreateInstance);
            else
                CreateInstance();

            driftThreshold = (uint)(systemSampleRate * DriftMS) / 1000;
            targetLatency = (uint)(systemSampleRate * LatencyMS) / 1000;
            adjustedLatency = targetLatency;
            actualLatency = (int)targetLatency;

            SetupCompressor();
        }

        private void SetupCompressor()
        {
            RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.COMPRESSOR, out compressorDSP);

            // Configuration du compresseur
            compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.THRESHOLD, -10.0f); // Seuil d'activation
            compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.RATIO, 4.0f); // Ratio de compression
            compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.ATTACK, 10.0f); // Temps d'attaque en ms
            compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.RELEASE, 200.0f); // Temps de relâchement en ms
            compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.GAINMAKEUP, 5.0f); // Gain de compensation

            //Debug.Log("Compresseur audio configuré");
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT AudioEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            var instance = new EventInstance(instancePtr);
            instance.getUserData(out var soundPtr);

            if (soundPtr == IntPtr.Zero) return RESULT.OK;

            var soundHandle = GCHandle.FromIntPtr(soundPtr);
            var sound = (Sound)soundHandle.Target;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    var parameter =
                        (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
                            typeof(PROGRAMMER_SOUND_PROPERTIES));
                    parameter.sound = sound.handle;
                    parameter.subsoundIndex = -1;
                    Marshal.StructureToPtr(parameter, parameterPtr, false);
                    break;

                case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                    sound.release();
                    break;

                case EVENT_CALLBACK_TYPE.DESTROYED:
                    soundHandle.Free();
                    break;
            }

            return RESULT.OK;
        }

        private void CreateInstance()
        {
            AudioInstance = AudioManager.CreateAudioInstance(audioModel);

            if (!AudioManager.TryGetEventInstance(AudioInstance.ID, out var eventInstance))
                //Debug.LogError("AudioInstance pour VivoxParticipant non créé : " + AudioInstance.ID);
                return;

            this.eventInstance = eventInstance;
            audioCallback = AudioEventCallback;
            this.eventInstance.setCallback(audioCallback);
            this.eventInstance.start();
            AudioManager.AttachInstanceToGameObject(AudioInstance.ID, transform);
        }

        private void UpdateBufferLatency(uint samplesWritten)
        {
            totalSamplesWritten += samplesWritten;

            if (samplesWritten != 0 && samplesWritten < minimumSamplesWritten)
            {
                minimumSamplesWritten = samplesWritten;
                adjustedLatency = Math.Max(samplesWritten, targetLatency);
            }

            var latency = (int)totalSamplesWritten - (int)totalSamplesRead;
            actualLatency = (int)(0.93f * actualLatency + 0.03f * latency);

            if (!channel.hasHandle()) return;

            var playbackRate = systemSampleRate;
            if (actualLatency < (int)(adjustedLatency - driftThreshold))
                playbackRate -= (int)(systemSampleRate * (DriftCorrectionPercentage / 100.0f));
            else if (actualLatency > (int)(adjustedLatency + driftThreshold))
                playbackRate += (int)(systemSampleRate * (DriftCorrectionPercentage / 100.0f));

            channel.setFrequency(playbackRate);
        }


        private void ProcessAudio(int channels)
        {
            if (!channel.hasHandle() || audioBuffer.Count == 0) return;

            channel.getPosition(out var readPosition, TIMEUNIT.PCMBYTES);
            var bytesRead = readPosition <= bufferReadPosition
                ? readPosition + soundInfo.length - bufferReadPosition
                : readPosition - bufferReadPosition;

            if (bytesRead <= 0 || audioBuffer.Count < bytesRead) return;

            sound.@lock(bufferReadPosition, bytesRead, out var ptr1, out var ptr2, out var len1, out var len2);
            var tmpBuffer = new float[(len1 + len2) / sizeof(float)];

            audioBuffer.CopyTo(0, tmpBuffer, 0, tmpBuffer.Length);
            audioBuffer.RemoveRange(0, tmpBuffer.Length);

            if (len1 > 0) Marshal.Copy(tmpBuffer, 0, ptr1, (int)len1 / sizeof(float));
            if (len2 > 0) Marshal.Copy(tmpBuffer, (int)len1 / sizeof(float), ptr2, (int)len2 / sizeof(float));

            sound.unlock(ptr1, ptr2, len1, len2);

            bufferReadPosition = readPosition;
            totalSamplesRead += (uint)tmpBuffer.Length;
        }
    }
}