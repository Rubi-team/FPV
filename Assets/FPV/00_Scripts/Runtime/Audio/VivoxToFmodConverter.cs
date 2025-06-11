using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Audio;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.Services.Vivox.AudioTaps;
using UnityEngine;
using AudioSettings = UnityEngine.AudioSettings;
using Debug = UnityEngine.Debug;

public class VivoxToFmodConverter : MonoBehaviour
{
    private const int LatencyMS = 50;
    private const int DriftMS = 1;
    private const float DriftCorrectionPercentage = 0.5f;

    private AudioModel _audioModel;

    private int _systemSampleRate;
    public EventInstance _eventInstance { private set; get; }
    private EVENT_CALLBACK _audioCallback;

    private CREATESOUNDEXINFO _soundInfo;
    private Sound _sound;
    private Channel _channel;

    private VivoxCaptureSourceTap _channelAudioTap;

    private readonly List<float> _audioBuffer = new();
    private uint _bufferSamplesWritten;
    private uint _bufferReadPosition;
    private uint _driftThreshold;
    private uint _targetLatency;
    private uint _adjustedLatency;
    private int _actualLatency;
    private uint _totalSamplesWritten;
    private uint _totalSamplesRead;
    private uint _minimumSamplesWritten = uint.MaxValue;

    private bool _isSpeaking;

    public AudioInstance AudioInstance { private set; get; }

    private void Start()
    {
        //GetComponent<SteamAudioSource>().pathingProbeBatch = Find

        _channelAudioTap = GetComponent<VivoxCaptureSourceTap>();

        var audioModel = new AudioModel
        {
            EventName = "event:/VOIP"
        };

        Setup(audioModel);
    }

    private void Update()
    {
        //_channelAudioTap.
    }


    public void Setup(AudioModel audioModel)
    {
        _audioModel = audioModel;
        _systemSampleRate = AudioSettings.outputSampleRate;


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

        if (soundPtr == IntPtr.Zero)
        {
            if (type == EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND)
                Debug.LogWarning("Sound pointer is null in CREATE_PROGRAMMER_SOUND callback");
            return RESULT.OK;
        }

        try
        {
            var soundHandle = GCHandle.FromIntPtr(soundPtr);
            if (!soundHandle.IsAllocated)
            {
                Debug.LogWarning("Sound handle is not allocated");
                return RESULT.OK;
            }

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

                    // Assurez-vous que sound.handle est valide avant de tenter de libérer
                    if (sound.handle != IntPtr.Zero) sound.release();

                    // Vérifiez si le son dans le paramètre est valide
                    if (parameter.sound != IntPtr.Zero)
                    {
                        sound = new Sound(parameter.sound);
                        sound.release();
                    }

                    break;
                }
                case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    if (soundHandle.IsAllocated) soundHandle.Free();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in AudioEventCallback: {e.Message}");
        }

        return RESULT.OK;
    }

    private void CreateInstance()
    {
        AudioInstance = AudioManager.CreateAudioInstance(_audioModel);

        if (!AudioManager.TryGetEventInstance(AudioInstance.ID, out var eventInstance))
        {
            Debug.LogError("Failed to get event instance from AudioManager.");
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

    private void ProcessAudio(int channels)
    {
        // Vérifier si le canal est valide
        var channelValid = _channel.hasHandle();

        if (!channelValid)
        {
            if (!_isSpeaking) return;

            var result = _eventInstance.getChannelGroup(out var channelGroup);
            if (result != RESULT.OK)
            {
                Debug.LogError("Error getting channel group: " + result);
                return;
            }

            _soundInfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
            _soundInfo.numchannels = channels;
            _soundInfo.defaultfrequency = _systemSampleRate;
            _soundInfo.length = _targetLatency * (uint)channels * sizeof(float);
            _soundInfo.format = SOUND_FORMAT.PCMFLOAT;

            var soundResult = RuntimeManager.CoreSystem.createSound("voip", MODE.LOOP_NORMAL | MODE.OPENUSER,
                ref _soundInfo, out _sound);
            if (soundResult != RESULT.OK)
            {
                Debug.LogError("Error creating sound: " + soundResult);
                return;
            }

            var playResult = RuntimeManager.CoreSystem.playSound(_sound, channelGroup, false, out _channel);
            if (playResult != RESULT.OK)
            {
                Debug.LogError("Error playing sound: " + playResult);
                _sound.release();
                return;
            }

            var soundHandle = GCHandle.Alloc(_sound, GCHandleType.Pinned);
            _eventInstance.setUserData(GCHandle.ToIntPtr(soundHandle));
            _bufferReadPosition = 0;

            return;
        }

        if (_audioBuffer.Count == 0) return;

        // Au lieu de vérifier avec getLastError, on peut utiliser isValid() ou un try-catch
        var soundValid = true;
        try
        {
            // Essayer d'accéder à une propriété du son pour voir s'il est valide
            _sound.getMode(out _);
        }
        catch
        {
            soundValid = false;
        }

        if (!soundValid)
        {
            Debug.LogWarning("Sound no longer valid, recreating");
            if (_channel.hasHandle()) _channel.stop();
            _channel = new Channel();
            return;
        }

        var posResult = _channel.getPosition(out var readPosition, TIMEUNIT.PCMBYTES);
        if (posResult != RESULT.OK)
        {
            Debug.LogWarning("Error getting channel position: " + posResult);
            return;
        }

        var bytesRead = readPosition - _bufferReadPosition;
        if (readPosition <= _bufferReadPosition) bytesRead += _soundInfo.length;

        if (bytesRead <= 0 || _audioBuffer.Count < bytesRead) return;

        try
        {
            var res = _sound.@lock(_bufferReadPosition, bytesRead, out var ptr1, out var ptr2, out var len1,
                out var len2);
            if (res != RESULT.OK)
            {
                Debug.LogError("Error locking sound: " + res);
                return;
            }

            var sampleLen1 = (int)(len1 / sizeof(float));
            var sampleLen2 = (int)(len2 / sizeof(float));
            var samplesRead = sampleLen1 + sampleLen2;

            if (samplesRead > 0)
            {
                var tmpBuffer = new float[samplesRead];
                _audioBuffer.CopyTo(0, tmpBuffer, 0, Math.Min(tmpBuffer.Length, _audioBuffer.Count));
                _audioBuffer.RemoveRange(0, Math.Min(tmpBuffer.Length, _audioBuffer.Count));

                if (len1 > 0) Marshal.Copy(tmpBuffer, 0, ptr1, sampleLen1);
                if (len2 > 0) Marshal.Copy(tmpBuffer, sampleLen1, ptr2, sampleLen2);

                res = _sound.unlock(ptr1, ptr2, len1, len2);
                if (res != RESULT.OK)
                {
                    Debug.LogError("Error unlocking sound: " + res);
                    return;
                }

                _bufferReadPosition = readPosition;
                _totalSamplesRead += (uint)samplesRead;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception in ProcessAudio: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        if (_channel.hasHandle()) _channel.stop();

        if (_sound.handle != IntPtr.Zero) _sound.release();

        if (_eventInstance.handle != IntPtr.Zero) _eventInstance.release();
    }
}