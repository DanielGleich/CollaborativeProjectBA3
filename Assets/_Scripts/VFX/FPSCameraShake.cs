using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class FPSCameraShake : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    float originalAmplitude;
    float originalFrequency;

    private void Awake()
    {
        if (noise != null)
        { 
            originalAmplitude = noise.AmplitudeGain;
            originalFrequency = noise.FrequencyGain;
        }
    }

    public void TriggerShake(float duration, float amplitude, float frequency)
    {
        if (noise == null)
            return;

        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, amplitude, frequency));
    }

    IEnumerator ShakeRoutine(float duration, float amplitude, float frequency)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            noise.AmplitudeGain = Mathf.Lerp(amplitude, originalAmplitude, t);
            yield return null;
        }

        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
    }
}
