using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FMOD;
using FMODUnity;

public class VoiceChat : NetworkBehaviour
{
    public enum ChatType { Global, Proximity }
    public ChatType VoiceChatType = ChatType.Global;

    public enum DetectionType { PushToTalk, VoiceActivation }
    public DetectionType VoiceDetectionType = DetectionType.PushToTalk;

    public bool Activated = true;
    public KeyCode PushToTalkKey;

    public AudioSource source;
    public float proximityRange = 10f;
    public float voiceActivationThreshold = 0.002f;

    private bool canTalk = true;
    private bool previousCanTalk = false;

    private const int sampleRate = 48000;
    private const int bufferSize = 16384;

    private float[] audioBuffer;
    private float[] sampleData;
    private float[] micDataBuffer;

    private FMOD.System fmodSystem;
    private FMOD.Sound microphoneClip;
    private uint microphoneClipLength;
    private uint position;
    private int recordDeviceId = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
            return;

        if (source == null)
            UnityEngine.Debug.LogError("[VOICE] AudioSource not assigned!");

        fmodSystem = RuntimeManager.CoreSystem;

        recordDeviceId = MicrophoneManager.Instance.GetCurrentDeviceId();

        audioBuffer = new float[bufferSize];
        sampleData = new float[bufferSize];
        micDataBuffer = new float[bufferSize];
        source.playOnAwake = false;
    }

    void Update()
    {
        if (!Activated || !IsOwner)
            return;

        // TODO: Implement Input device change
        int selectedDevice = recordDeviceId;

        if (selectedDevice != recordDeviceId)
        {
            UpdateMicrophone(selectedDevice);
        }

        switch (VoiceDetectionType)
        {
            case DetectionType.PushToTalk:
                canTalk = Input.GetKey(PushToTalkKey);
                if (canTalk && !microphoneClip.hasHandle())
                {
                    StartMicrophone();
                    StartTalking();
                }
                else if (!canTalk && microphoneClip.hasHandle())
                {
                    StopTalking();
                    StopMicrophone();
                }
                break;

            case DetectionType.VoiceActivation:
                if (!microphoneClip.hasHandle())
                {
                    StartMicrophone();
                }
                canTalk = IsVoiceActivated();
                break;
        }

        if (!previousCanTalk && canTalk)
            StartTalking();

        if (previousCanTalk && !canTalk)
            StopTalking();

        previousCanTalk = canTalk;
    }

    private void StartMicrophone()
    {
        if (microphoneClip.hasHandle())
            StopMicrophone();

        CREATESOUNDEXINFO exInfo = new CREATESOUNDEXINFO();
        exInfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
        exInfo.numchannels = 1;
        exInfo.format = SOUND_FORMAT.PCM16;
        exInfo.defaultfrequency = sampleRate;
        exInfo.length = (uint)(sampleRate * sizeof(short) * 10); //Might change length from 10 to sth else?

        RESULT result = fmodSystem.createSound(
            "",
            MODE.LOOP_NORMAL | MODE.OPENUSER,
            ref exInfo,
            out microphoneClip
        );

        if (result != RESULT.OK)
        {
            UnityEngine.Debug.LogError("[VOICE] FMOD createSound failed: " + result);
            microphoneClip.clearHandle();
            return;
        }

        result = fmodSystem.recordStart(recordDeviceId, microphoneClip, true);
        if (result != RESULT.OK)
        {
            UnityEngine.Debug.LogError("[VOICE] FMOD recordStart failed: " + result);
            microphoneClip.release();
            microphoneClip.clearHandle();
            return;
        }

        microphoneClip.getLength(out microphoneClipLength, TIMEUNIT.PCM);
        position = 0;
    }

    private void StopMicrophone()
    {
        if (!microphoneClip.hasHandle())
            return;

        fmodSystem.recordStop(recordDeviceId);
        microphoneClip.release();
        microphoneClip.clearHandle();
    }

    private void UpdateMicrophone(int newDriverId)
    {
        if (recordDeviceId == newDriverId)
            return;

        UnityEngine.Debug.Log($"[VOICE] Switching FMOD record driver from {recordDeviceId} to {newDriverId}");

        bool wasTalking = canTalk;

        StopTalking();
        StopMicrophone();

        recordDeviceId = newDriverId;

        if (wasTalking)
        {
            StartMicrophone();
            StartTalking();
        }
    }

    private void StartTalking()
    {
        UnityEngine.Debug.Log("Talk Start");
        if (!microphoneClip.hasHandle())
            return;

        StartCoroutine(TransmitVoice());
    }

    private void StopTalking()
    {
        UnityEngine.Debug.Log("Talk Stop");
        StopAllCoroutines();
    }

    private IEnumerator TransmitVoice()
    {
        while (canTalk)
        {
            if (!microphoneClip.hasHandle()) yield break;
            fmodSystem.update();

            uint recordPos;
            if (fmodSystem.getRecordPosition(recordDeviceId, out recordPos) != RESULT.OK)
            {
                yield return null; continue;
            }

            uint recordDelta = (recordPos >= position)
                ? (recordPos - position)
                : (recordPos + microphoneClipLength - position);

            if (recordDelta < bufferSize)
            {
                yield return null; continue;
            }

            uint byteOffset = position * 2u;
            uint byteLength = (uint)bufferSize * 2u;

            IntPtr ptr1, ptr2; uint len1, len2;
            if (microphoneClip.@lock(byteOffset, byteLength, out ptr1, out ptr2, out len1, out len2) != RESULT.OK)
            {
                yield return null; continue;
            }

            int sampleCount1 = (int)(len1 / 2u);   // actual valid samples we read

            ReadFmodBuffer(ptr1, sampleCount1, audioBuffer);

            microphoneClip.unlock(ptr1, ptr2, len1, len2);
            position = (position + (uint)bufferSize) % microphoneClipLength;

            if (sampleCount1 > 0)
                TransmitAudioServerRpc(audioBuffer, sampleCount1);

            yield return new WaitForSeconds(bufferSize / (float)sampleRate);
        }
    }



    private bool IsVoiceActivated()
    {
        if (!microphoneClip.hasHandle())
            return false;

        fmodSystem.update();

        uint recordPos;
        if (fmodSystem.getRecordPosition(recordDeviceId, out recordPos) != RESULT.OK)
            return false;

        uint recordDelta = (recordPos >= position)
            ? (recordPos - position)
            : (recordPos + microphoneClipLength - position);

        if (recordDelta < bufferSize)
            return false;

        uint byteOffset = ((recordPos + microphoneClipLength - (uint)bufferSize) % microphoneClipLength) * 2u;
        uint byteLength = (uint)bufferSize * 2u;

        IntPtr ptr1, ptr2;
        uint len1, len2;

        if (microphoneClip.@lock(byteOffset, byteLength, out ptr1, out ptr2, out len1, out len2) != RESULT.OK)
            return false;

        int sampleCount1 = (int)(len1 / 2u);
        ReadFmodBuffer(ptr1, sampleCount1, sampleData);

        microphoneClip.unlock(ptr1, ptr2, len1, len2);

        float sum = 0f;
        for (int i = 0; i < sampleCount1; i++)
            sum += Mathf.Abs(sampleData[i]);

        float average = sum / sampleCount1;
        return average > voiceActivationThreshold;
    }


    [ServerRpc(RequireOwnership = false)]
    private void TransmitAudioServerRpc(float[] audioData, int validSamples, NetworkConnection sender = null)
    {
        TransmitAudioObserversRpc(audioData, validSamples, sender.ClientId);
    }

    [ObserversRpc]
    private void TransmitAudioObserversRpc(float[] audioData, int validSamples, int senderClientId)
    {
        if (senderClientId == NetworkManager.ClientManager.Connection.ClientId)
            return;

        PlayReceivedAudio(audioData, validSamples, senderClientId);
    }


    private void PlayReceivedAudio(float[] audioData, int validSamples, int senderClientId)
    {
        if (source == null)
        {
            UnityEngine.Debug.LogError("[VOICE] AudioSource not assigned!");
            return;
        }

        // Set spatial blend based on chat type
        if (VoiceChatType == ChatType.Proximity)
        {
            source.spatialBlend = 1.0f; // Make the audio 3D
            source.maxDistance = proximityRange;
            Transform senderTransform = GetPlayerTransform(senderClientId);
            if (senderTransform != null)
            {
                float distance = Vector3.Distance(transform.position, senderTransform.position);
                if (distance > proximityRange)
                    return;
            }
        }
        else
        {
            source.spatialBlend = 0.0f; // Make the audio 2D for global chat
        }

        if (validSamples <= 0)
            return;

        AudioClip clip = AudioClip.Create("ReceivedVoice", validSamples, 1, sampleRate, false);
        clip.SetData(audioData, 0);

        source.clip = clip;
        source.Play();
    }

    private Transform GetPlayerTransform(int clientId)
    {
        foreach (var obj in FindObjectsOfType<NetworkObject>())
        {
            if (obj.Owner.ClientId == clientId)
            {
                return obj.transform;
            }
        }
        return null;
    }

    private float GetMicInputVolume()
    {
        if (!microphoneClip.hasHandle())
            return 0f;

        fmodSystem.update();

        uint recordPos;
        if (fmodSystem.getRecordPosition(recordDeviceId, out recordPos) != RESULT.OK)
            return 0f;

        uint recordDelta = (recordPos >= position)
            ? (recordPos - position)
            : (recordPos + microphoneClipLength - position);

        if (recordDelta < bufferSize)
            return 0f;

        uint byteOffset = ((recordPos + microphoneClipLength - (uint)bufferSize) % microphoneClipLength) * 2u;
        uint byteLength = (uint)bufferSize * 2u;

        IntPtr ptr1, ptr2;
        uint len1, len2;

        if (microphoneClip.@lock(byteOffset, byteLength, out ptr1, out ptr2, out len1, out len2) != RESULT.OK)
            return 0f;

        int sampleCount1 = (int)(len1 / 2u);
        ReadFmodBuffer(ptr1, sampleCount1, micDataBuffer);

        microphoneClip.unlock(ptr1, ptr2, len1, len2);

        float sum = 0f;
        for (int i = 0; i < sampleCount1; i++)
            sum += micDataBuffer[i] * micDataBuffer[i];

        float rmsValue = Mathf.Sqrt(sum / sampleCount1);
        float amplifiedVolume = Mathf.Clamp(rmsValue * 50f, 0f, 1f);
        return amplifiedVolume;
    }

    private unsafe void ReadFmodBuffer(IntPtr ptr, int sampleCount, float[] targetBuffer)
    {
        short* src = (short*)ptr.ToPointer();
        for (int i = 0; i < sampleCount; ++i)
            targetBuffer[i] = src[i] / 32768.0f;
    }
}
