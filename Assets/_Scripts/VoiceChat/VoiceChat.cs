using FishNet.Connection;
using FishNet.Object;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.AudioClip;

public class VoiceChat : NetworkBehaviour
{
    public enum ChatType { Global, Proximity }
    public ChatType VoiceChatType = ChatType.Global;

    public enum DetectionType { PushToTalk, VoiceActivation }
    public DetectionType VoiceDetectionType = DetectionType.PushToTalk;

    public bool Activated = true;
    public KeyCode PushToTalkKey;

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

    private EventInstance voiceEventTemplate;
    private Dictionary<int, FMOD.Sound> playerVoiceSounds = new Dictionary<int, FMOD.Sound>();
    private Dictionary<int, FMOD.Channel> playerVoiceChannels = new Dictionary<int, FMOD.Channel>();
    ChannelGroup voiceChatGroup;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
            return;

        fmodSystem = RuntimeManager.CoreSystem;

        recordDeviceId = MicrophoneManager.Instance.GetCurrentDeviceId();

        audioBuffer = new float[bufferSize];
        sampleData = new float[bufferSize];
        micDataBuffer = new float[bufferSize];
        
        fmodSystem.getMasterChannelGroup(out FMOD.ChannelGroup masterGroup);
        fmodSystem.createChannelGroup("VoiceChat", out voiceChatGroup);
        masterGroup.addGroup(voiceChatGroup);

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

            int sampleCount1 = (int)(len1 / 2u);

            Array.Clear(audioBuffer, 0, sampleCount1);
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
        if (validSamples <= 0) return;

        if (VoiceChatType == ChatType.Proximity)
        {
            Transform senderTransform = GetPlayerTransform(senderClientId);
            if (senderTransform == null) return;

            float distance = Vector3.Distance(transform.position, senderTransform.position);
            if (distance > proximityRange)
            {
                StopVoiceChannel(senderClientId);
                return;
            }
        }

        FMOD.Sound voiceSound;
        CreateFmodVoiceSound(audioData, validSamples, out voiceSound);

        FMOD.Channel voiceChannel;
        fmodSystem.playSound(voiceSound, voiceChatGroup, false, out voiceChannel);

        playerVoiceSounds[senderClientId] = voiceSound;
        playerVoiceChannels[senderClientId] = voiceChannel;

        if (VoiceChatType == ChatType.Proximity)
        {
            Transform senderTransform = GetPlayerTransform(senderClientId);
            FMOD.VECTOR pos = new FMOD.VECTOR { x = senderTransform.position.x, y = senderTransform.position.y, z = senderTransform.position.z };
            FMOD.VECTOR vel = new FMOD.VECTOR { x = 0, y = 0, z = 0 };
            voiceChannel.set3DAttributes(ref pos, ref vel);
            voiceChannel.set3DMinMaxDistance(1.0f, proximityRange);
        }
    }

    private void CreateFmodVoiceSound(float[] audioData, int validSamples, out FMOD.Sound sound)
    {
        CREATESOUNDEXINFO exInfo = new CREATESOUNDEXINFO();
        exInfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
        exInfo.numchannels = 1;
        exInfo.format = SOUND_FORMAT.PCMFLOAT;
        exInfo.defaultfrequency = sampleRate;
        exInfo.length = (uint)(validSamples * sizeof(float));

        RESULT result = fmodSystem.createSound("", MODE.DEFAULT, ref exInfo, out sound);
        if (result != RESULT.OK)
        {
            UnityEngine.Debug.LogError("[VOICE] FMOD createSound failed: " + result);
            return;
        }

        sound.@lock(0, exInfo.length, out IntPtr ptr, out _, out uint len, out _);
        Marshal.Copy(audioData, 0, ptr, validSamples);
        sound.unlock(ptr, IntPtr.Zero, len, 0);
    }

    private void StopVoiceChannel(int clientId)
    {
        if (playerVoiceChannels.TryGetValue(clientId, out FMOD.Channel channel))
        {
            channel.stop();
            playerVoiceChannels.Remove(clientId);
        }

        if (playerVoiceSounds.TryGetValue(clientId, out FMOD.Sound sound))
        {
            sound.release();
            playerVoiceSounds.Remove(clientId);
        }
    }

    private Transform GetPlayerTransform(int clientId)
    {
        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj.Owner.ClientId == clientId)
            {
                return obj.transform;
            }
        }
        return null;
    }

    private unsafe void ReadFmodBuffer(IntPtr ptr, int sampleCount, float[] targetBuffer)
    {
        short* src = (short*)ptr.ToPointer();
        for (int i = 0; i < sampleCount; ++i)
            targetBuffer[i] = src[i] / 32768.0f;
    }

    void OnDestroy()
    {
        foreach (var kvp in playerVoiceChannels)
            kvp.Value.stop();
        foreach (var kvp in playerVoiceSounds)
            kvp.Value.release();

        playerVoiceChannels.Clear();
        playerVoiceSounds.Clear();
    }

}
