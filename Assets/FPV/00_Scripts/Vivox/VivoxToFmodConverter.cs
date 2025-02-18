using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Audio;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Vivox
{
	public class VivoxToFmodConverter : MonoBehaviour
	{
		private const int LatencyMS = 50;
		private const int DriftMS = 1;
		private const float DriftCorrectionPercentage = 0.5f;

		private AudioModel audioModel;

		private int systemSampleRate;
		private EventInstance eventInstance;
		private EVENT_CALLBACK audioCallback;

		private CREATESOUNDEXINFO soundInfo;
		private Sound sound;
		private Channel channel;

		private readonly List<float> audioBuffer = new();
		private uint bufferSamplesWritten;
		private uint bufferReadPosition;
		private uint driftThreshold;
		private uint targetLatency;
		private uint adjustedLatency;
		private int actualLatency;
		private uint totalSamplesWritten;
		private uint totalSamplesRead;
		private uint minimumSamplesWritten = uint.MaxValue;

		private bool isSpeaking;
		
		[SerializeField] private LogHandler logHandler;

		private AudioInstance AudioInstance { set; get; }

		public void Setup(AudioModel audioModelSetup)
		{
			this.audioModel = audioModelSetup;
			systemSampleRate = AudioSettings.outputSampleRate;

			if (!AudioBankLoader.HasBankLoaded(this.audioModel.Bank))
			{
				AudioBankLoader.LoadBank(this.audioModel.Bank, true, CreateInstance);
			}
			else
			{
				CreateInstance();
			}

			driftThreshold = (uint)(systemSampleRate * DriftMS) / 1000;
			targetLatency = (uint)(systemSampleRate * LatencyMS) / 1000;
			adjustedLatency = targetLatency;
			actualLatency = (int)targetLatency;
		}

		[MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
		private static RESULT AudioEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
		{
			var instance = new EventInstance(instancePtr);
			instance.getUserData(out IntPtr soundPtr);

			if (soundPtr == IntPtr.Zero) return RESULT.OK;

			var soundHandle = GCHandle.FromIntPtr(soundPtr);
			var sound = (Sound)soundHandle.Target;

			switch (type)
			{
				case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
				{
					var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
						typeof(PROGRAMMER_SOUND_PROPERTIES));
					parameter.sound = sound.handle;
					parameter.subsoundIndex = -1;
					Marshal.StructureToPtr(parameter, parameterPtr, false);
					break;
				}
				case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
				{
					var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
						typeof(PROGRAMMER_SOUND_PROPERTIES));
					sound.release();
					sound = new(parameter.sound);
					sound.release();
					break;
				}
				case EVENT_CALLBACK_TYPE.DESTROYED:
				{
					soundHandle.Free();
					break;
				}
			}

			return RESULT.OK;
		}

		private void CreateInstance()
		{
			AudioInstance = AudioManager.CreateAudioInstance(audioModel);

			if (!AudioManager.TryGetEventInstance(AudioInstance.ID, out EventInstance eventInstance))
			{
				logHandler?.Error("AudioInstance for VivoxParticipant has not being created:" + AudioInstance.ID, this);
				return;
			}

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

			int latency = (int)totalSamplesWritten - (int)totalSamplesRead;
			actualLatency = (int)(0.93f * actualLatency + 0.03f * latency);

			if (!channel.hasHandle()) return;

			int playbackRate = systemSampleRate;
			if (actualLatency < (int)(adjustedLatency - driftThreshold))
			{
				playbackRate = systemSampleRate - (int)(systemSampleRate * (DriftCorrectionPercentage / 100.0f));
			}
			else if (actualLatency > (int)(adjustedLatency + driftThreshold))
			{
				playbackRate = systemSampleRate + (int)(systemSampleRate * (DriftCorrectionPercentage / 100.0f));
			}

			channel.setFrequency(playbackRate);
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channel.hasHandle())
			{
				audioBuffer.AddRange(data);
				UpdateBufferLatency((uint)data.Length);
			}

			isSpeaking = false;
			foreach (float value in data)
			{
				if (value == 0) continue;

				isSpeaking = true;
				break;
			}

			ProcessAudio(channels);

			for (int i = 0; i < data.Length; i++)
			{
				data[i] = 0;
			}
		}

		private void ProcessAudio(int channels)
		{
			if (!channel.hasHandle())
			{
				if (!isSpeaking) return;

				RESULT result = eventInstance.getChannelGroup(out ChannelGroup channelGroup);
				if (result != RESULT.OK)
				{
					logHandler?.Error(result.ToString(), this);
				}

				soundInfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
				soundInfo.numchannels = channels;
				soundInfo.defaultfrequency = systemSampleRate;
				soundInfo.length = targetLatency * (uint)channels * sizeof(float);
				soundInfo.format = SOUND_FORMAT.PCMFLOAT;

				RuntimeManager.CoreSystem.createSound("voip", MODE.LOOP_NORMAL | MODE.OPENUSER, ref soundInfo,
					out sound);
				RuntimeManager.CoreSystem.playSound(sound, channelGroup, false, out channel);

				return;
			}

			if (audioBuffer.Count == 0) return;

			channel.getPosition(out uint readPosition, TIMEUNIT.PCMBYTES);

			uint bytesRead = readPosition - bufferReadPosition;
			if (readPosition <= bufferReadPosition)
			{
				bytesRead += soundInfo.length;
			}

			if (bytesRead <= 0 || audioBuffer.Count < bytesRead) return;

			RESULT res = sound.@lock(bufferReadPosition, bytesRead, out IntPtr ptr1, out IntPtr ptr2, out uint len1,
				out uint len2);
			if (res != RESULT.OK)
			{
				logHandler?.Error(res.ToString(), this);
			}

			// Though soundInfo.format is float, data retrieved from Sound::lock is in bytes,
			// so we only copy (len1+len2)/sizeof(float) full float values across
			int sampleLen1 = (int)(len1 / sizeof(float));
			int sampleLen2 = (int)(len2 / sizeof(float));
			int samplesRead = sampleLen1 + sampleLen2;
			float[] tmpBuffer = new float[samplesRead];

			audioBuffer.CopyTo(0, tmpBuffer, 0, tmpBuffer.Length);
			audioBuffer.RemoveRange(0, tmpBuffer.Length);

			if (len1 > 0)
			{
				Marshal.Copy(tmpBuffer, 0, ptr1, sampleLen1);
			}
			if (len2 > 0)
			{
				Marshal.Copy(tmpBuffer, sampleLen1, ptr2, sampleLen2);
			}

			res = sound.unlock(ptr1, ptr2, len1, len2);
			if (res != RESULT.OK)
			{
				logHandler?.Error(res.ToString(), this);
			}

			bufferReadPosition = readPosition;
			totalSamplesRead += (uint)samplesRead;

			var soundHandle = GCHandle.Alloc(sound, GCHandleType.Pinned);
			eventInstance.setUserData(GCHandle.ToIntPtr(soundHandle));
		}

		private void OnDestroy()
		{
			sound.release();
		}
	}
}