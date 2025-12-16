using UnityEngine;
using UnityEngine.UI;

public class UISteamProfileValidation : UISteamProfileValidationDisplay
{
    [SerializeField] private GameObject profileParent;
    private LayoutElement layoutElement;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    protected override void OnProfileUpdateValid()
    { 
        profileParent.SetActive(true);
        layoutElement.ignoreLayout = false;
    }

    protected override void OnProfileUpdateInvalid()
    { 
        profileParent.SetActive(false);
        layoutElement.ignoreLayout = true;
    }
}
