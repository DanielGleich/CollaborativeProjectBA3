using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component used to set the volume of different FMOD busses
/// </summary>
public class FMODMixerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text sliderName;
    [SerializeField] private Slider slider;

    [Header("Settings")]
    [Tooltip("The path of the bus to control without the 'bus:/' prefix. An empty string is the Master Bus")]
    [SerializeField] private string busPath;
    [SerializeField] private string sliderNameText;

    private FMOD.Studio.Bus bus;

    void OnValidate()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>();
        if (sliderName == null)
            sliderName = GetComponentInChildren<TMP_Text>();
        if (sliderName != null)
            sliderName.text = sliderNameText;
    }
    void Awake()
    {
        bus = RuntimeManager.GetBus("bus:/" + busPath);
        SetUpSlider();
    }
    private void SetUpSlider()
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(UpdateVolume);
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;
    }
    void Start()
    {
        bus = RuntimeManager.GetBus("bus:/" + busPath);
    }
    void OnEnable()
    {
        bus.getVolume(out float volume);
        slider.value = volume;
    }
    private void UpdateVolume(float value)
    {
        bus.setVolume(value);
    }
}
