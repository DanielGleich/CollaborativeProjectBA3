using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionDropDown : MonoBehaviour {
    private int currentResolutionIndex;
    private Resolution[] resolutions;
    [Header("References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    void OnValidate()
    {
        if (resolutionDropdown == null)
            resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();
        if (resolutionDropdown != null)
            SetUpDropdown();
    }

    private void SetUpDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    void OnEnable()
    {
        SetUpDropdown();
        SetResolution(currentResolutionIndex);
    }
    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width,resolutions[index].height,Screen.fullScreen);
    }
}
