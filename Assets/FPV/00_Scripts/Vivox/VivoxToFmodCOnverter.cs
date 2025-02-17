using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils;

public class VivoxToFmodConverter : MonoBehaviour
{
    [Header("Debug")] [SerializeField]
    private LogHandler _logHandler;
    
    private const int LatencyMS = 50;
    private const int DriftMS = 1;
    private const float DriftCorrectionPercentage = 0.5f;

    private readonly List<float> _audioBuffer = new();
    private int _actualLatency;
    private uint _adjustedLatency;
    private EVENT_CALLBACK _audioCallback;

    private AudioModel _audioModel;
    private uint _bufferReadPosition;
    private uint _bufferSamplesWritten;
    private Channel _channel;
    private uint _driftThreshold;
    private EventInstance _eventInstance;

    private bool _isSpeaking;

    
    private uint _minimumSamplesWritten = uint.MaxValue;
    private Sound _sound;

    private CREATESOUNDEXINFO _soundInfo;

    private int _systemSampleRate;
    private uint _targetLatency;
    private uint _totalSamplesRead;
    private uint _totalSamplesWritten;

    public AudioInstance AudioInstance { private set; get; }


    private void Awake()
    {
        var eventList = RuntimeManager.StudioSystem;
        Bank[] banks;
        eventList.getBankList(out banks);

        foreach (var bank in banks)
        {
            EventDescription[] events;
            bank.getEventList(out events);

            foreach (var ev in events)
            {
                ev.getPath(out var path);
                _logHandler.Log("FMOD Event: " + path);
            }
        }


        // Create a new AudioModel instance with default values
        var audioModel = new AudioModel
        {
            Bank = "Master", // Replace with the actual FMOD bank name
            EventName = "event:/VIVOX/Voip" // Replace with your actual FMOD event
        };

        // Call Setup with the created AudioModel
        Setup(audioModel);
    }


    private void OnDestroy()
    {
        _sound.release();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (_channel.hasHandle())
        {
            _audioBuffer.AddRange(data);
            UpdateBufferLatency((uint)data.Length);
        }

        _isSpeaking = false;
        foreach (var value in data)
        {
            if (value == 0) continue;

            _isSpeaking = true;
            break;
        }

        ProcessAudio(channels);

        for (var i = 0; i < data.Length; i++) data[i] = 0;
    }

    public void Setup(AudioModel audioModel)
    {
        _audioModel = audioModel;
        _systemSampleRate = AudioSettings.outputSampleRate;
        if (_systemSampleRate <= 0)
        {
            _logHandler.Warning("AudioSettings.outputSampleRate returned 0, defaulting to 48000Hz", this);
            _systemSampleRate = 48000;
        }

        if (!AudioBankLoader.HasBankLoaded(_audioModel.Bank))
            AudioBankLoader.LoadBank(_audioModel.Bank, true, CreateInstance);
        else
            CreateInstance();

        _driftThreshold = (uint)(_systemSampleRate * DriftMS) / 1000;
        _targetLatency = (uint)(_systemSampleRate * LatencyMS) / 1000;
        _adjustedLatency = _targetLatency;
        _actualLatency = (int)_targetLatency;
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
                sound = new Sound(parameter.sound);
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
        AudioInstance = AudioManager.CreateAudioInstance(_audioModel);

        if (!AudioManager.TryGetEventInstance(AudioInstance.ID, out var eventInstance))
        {
            _logHandler.Error("AudioInstance for VivoxParticipant has not being created:" + AudioInstance.ID,
                this);
            return;
        }

        _eventInstance = eventInstance;
        _audioCallback = AudioEventCallback;
        _eventInstance.setCallback(_audioCallback);

        _eventInstance.start();
        AudioManager.AttachInstanceToGameObject(AudioInstance.ID, transform);
    }

    private void UpdateBufferLatency(uint samplesWritten)
    {
        _totalSamplesWritten += samplesWritten;

        if (samplesWritten != 0 && samplesWritten < _minimumSamplesWritten)
        {
            _minimumSamplesWritten = samplesWritten;
            _adjustedLatency = Math.Max(samplesWritten, _targetLatency);
        }

        var latency = (int)_totalSamplesWritten - (int)_totalSamplesRead;
        _actualLatency = (int)(0.93f * _actualLatency + 0.03f * latency);

        if (!_channel.hasHandle()) return;

        var playbackRate = _systemSampleRate;
        if (_actualLatency < (int)(_adjustedLatency - _driftThreshold))
            playbackRate = _systemSampleRate - (int)(_systemSampleRate * (DriftCorrectionPercentage / 100.0f));
        else if (_actualLatency > (int)(_adjustedLatency + _driftThreshold))
            playbackRate = _systemSampleRate + (int)(_systemSampleRate * (DriftCorrectionPercentage / 100.0f));

        _channel.setFrequency(playbackRate);
    }

    private void ProcessAudio(int channels)
    {
        if (!_channel.hasHandle())
        {
            if (!_isSpeaking) return;

            ChannelGroup channelGroup;

            if (!_eventInstance.isValid())
            {
                _logHandler?.Error("Event instance is not valid.", this);
                // Fallback: use master channel group
                var masterResult = RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
                if (masterResult != RESULT.OK)
                {
                    _logHandler?.Error("Could not get master channel group: " + masterResult, this);
                    return;
                }
            }
            else
            {
                var result = _eventInstance.getChannelGroup(out channelGroup);
                if (result != RESULT.OK || !channelGroup.hasHandle())
                {
                    _logHandler?.Error("Event instance getChannelGroup returned error: " + result, this);
                    // Fallback: use master channel group
                    var masterResult = RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
                    if (masterResult != RESULT.OK)
                    {
                        _logHandler?.Error("Could not get master channel group: " + masterResult, this);
                        return;
                    }
                }
            }

            _soundInfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
            _soundInfo.numchannels = channels;
            _soundInfo.defaultfrequency = _systemSampleRate; // now guaranteed nonzero
            _soundInfo.length = _targetLatency * (uint)channels * sizeof(float);
            _soundInfo.format = SOUND_FORMAT.PCMFLOAT;

            var createResult = RuntimeManager.CoreSystem.createSound("voip", MODE.LOOP_NORMAL | MODE.OPENUSER,
                ref _soundInfo, out _sound);
            if (createResult != RESULT.OK)
            {
                _logHandler?.Error("Error creating FMOD sound: " + createResult, this);
                return;
            }

            var playResult = RuntimeManager.CoreSystem.playSound(_sound, channelGroup, false, out _channel);
            if (playResult != RESULT.OK) _logHandler?.Error("Error playing FMOD sound: " + playResult, this);
            return;
        }

        if (_audioBuffer.Count == 0) return;

        _channel.getPosition(out var readPosition, TIMEUNIT.PCMBYTES);

        var bytesRead = readPosition - _bufferReadPosition;
        if (readPosition <= _bufferReadPosition)
            bytesRead += _soundInfo.length;

        if (bytesRead <= 0 || _audioBuffer.Count < bytesRead)
            return;

        var res = _sound.@lock(_bufferReadPosition, bytesRead, out var ptr1, out var ptr2, out var len1, out var len2);
        if (res != RESULT.OK)
        {
            _logHandler?.Error(res.ToString(), this);
            return;
        }

        var sampleLen1 = (int)(len1 / sizeof(float));
        var sampleLen2 = (int)(len2 / sizeof(float));
        var samplesRead = sampleLen1 + sampleLen2;
        var tmpBuffer = new float[samplesRead];

        _audioBuffer.CopyTo(0, tmpBuffer, 0, tmpBuffer.Length);
        _audioBuffer.RemoveRange(0, tmpBuffer.Length);

        if (len1 > 0)
            Marshal.Copy(tmpBuffer, 0, ptr1, sampleLen1);
        if (len2 > 0)
            Marshal.Copy(tmpBuffer, sampleLen1, ptr2, sampleLen2);

        res = _sound.unlock(ptr1, ptr2, len1, len2);
        if (res != RESULT.OK) _logHandler?.Error(res.ToString(), this);

        _bufferReadPosition = readPosition;
        _totalSamplesRead += (uint)samplesRead;

        var soundHandle = GCHandle.Alloc(_sound, GCHandleType.Pinned);
        _eventInstance.setUserData(GCHandle.ToIntPtr(soundHandle));
    }
}