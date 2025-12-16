using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    protected override bool _dontDestroyOnLoad { get; } = true;
    public TMP_Dropdown Dropdown;
    [SerializeField] private Dictionary<int, string> AvailableDevices = new Dictionary<int, string>();

    void Start()
    {
        RuntimeManager.CoreSystem.getRecordNumDrivers(out int numDrivers, out int numConnected);

        AvailableDevices.Clear();
        Dropdown.ClearOptions();

        for (int i = 0; i < numDrivers; i++)
        {
            RuntimeManager.CoreSystem.getRecordDriverInfo(
                i,
                out string inputName,
                256,
                out System.Guid guid,
                out int systemRate,
                out FMOD.SPEAKERMODE speakerMode,
                out int speakermodeChannels,
                out FMOD.DRIVER_STATE driverState);

            // Skip non-default or disconnected if you only want usable devices
            if ((driverState & FMOD.DRIVER_STATE.CONNECTED) == 0)
                continue;

            // Skip  loopback devices
            if (inputName != null && inputName.Contains("[loopback]"))
                continue;
            AvailableDevices.Add(i, inputName);
        }

        var options = new List<OptionData>();
        foreach (var device in AvailableDevices)
            options.Add(new OptionData(device.Value));

        Dropdown.AddOptions(options);
    }

    public int GetCurrentDeviceId()
    {
        foreach (KeyValuePair<int, string> inputDevice in AvailableDevices)
        {
            if (Dropdown.options[Dropdown.value].text == inputDevice.Value)
            {
                return inputDevice.Key;
            }
        }
        return -1;
    }
}
