using TMPro;
using UnityEngine;

public class UISteamProfileName : UISteamProfileNameDisplay
{
    [SerializeField] private TextMeshProUGUI profileTextField;

    protected override void UpdateNameDisplay(string name)
    { 
        profileTextField.text = name;
    }
}
