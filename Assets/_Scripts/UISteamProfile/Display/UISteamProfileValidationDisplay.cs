using UnityEngine;

public abstract class UISteamProfileValidationDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected UISteamProfile steamProfile;


    private void OnEnable()
    {
        steamProfile.OnProfileEmpty += OnProfileUpdateInvalid;
        steamProfile.OnProfileFilled += OnProfileUpdateValid;
    }

    private void OnDisable()
    {
        steamProfile.OnProfileEmpty -= OnProfileUpdateInvalid;
        steamProfile.OnProfileFilled -= OnProfileUpdateValid;        
    }

    protected abstract void OnProfileUpdateValid();
    protected abstract void OnProfileUpdateInvalid();
}
