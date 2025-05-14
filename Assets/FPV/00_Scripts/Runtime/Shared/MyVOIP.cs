using System;
using UnityEngine;

namespace FPV
{
    [RequireComponent(typeof(AudioSource))]
    public class MyVOIP : MonoBehaviour
    {
        public int sampleWindow = 64;
        public AudioClip microphoneClip;

        private void Start()
        {
            MicrophoneToAudioClip();
        }

        private void MicrophoneToAudioClip()
        {
            // Get the microphone input
            var microphone = Microphone.devices[0];
            microphoneClip = Microphone.Start(microphone, true, 20, AudioSettings.outputSampleRate);
        }

        public float GetLoudnessFromMicrophone()
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
        }

        private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
        {
            var startPosition = clipPosition - sampleWindow;

            if (startPosition < 0)
                return 0;

            var waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);


            // Compute Loudness
            var totalLoudness = 0f;

            for (var i = 0; i < waveData.Length; i++) totalLoudness += Mathf.Abs(waveData[i]);

            return totalLoudness / sampleWindow;
        }
    }
}