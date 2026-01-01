using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleGraphicsQualityController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    void OnValidate()
    {
        if (qualityDropdown == null)
            qualityDropdown = GetComponentInChildren<TMP_Dropdown>();
        if (qualityDropdown != null)
            SetUpDropdown();
    }
    void OnEnable()
    {
        SetUpDropdown();
    }
    private void SetUpDropdown()
    {
        var qualityNames = QualitySettings.names;
        List<string> qualityOptions = new List<string>();
        for (int i = 0; i < qualityNames.Length; i++)
        {
            qualityOptions.Add(qualityNames[i]);
        }
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        qualityDropdown.RefreshShownValue();
    }
    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
        Debug.Log(QualitySettings.names[quality]);
    }
}