using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ResolutionDropDown : MonoBehaviour {
    private int currentResolutionIndex = -1;
    private List<Resolution> resolutions;
    [Header("References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Formating")]
    [SerializeField] private string format = "[0] x [1]";
    void OnValidate()
    {
        if (resolutionDropdown == null)
            resolutionDropdown = GetComponentInChildren<TMP_Dropdown>();
        if (resolutionDropdown != null)
            SetUpDropdown();
    }
    void OnEnable()
    {
        SetUpDropdown();
        SetResolution(currentResolutionIndex);
    }
    private void SetUpDropdown()
    {
        resolutions = Screen.resolutions.DistinctBy(x => new Vector2Int(x.width, x.height)).Reverse().ToList();
        resolutions.RemoveAll(x => (float)x.width/ Screen.width != (float) x.height/ Screen.height);
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int index)
    {
        if(index < 0 || index >= resolutions.Count)
            return;
        Screen.SetResolution(resolutions[index].width,resolutions[index].height,Screen.fullScreenMode);
        resolutionDropdown.RefreshShownValue();
    }
}
