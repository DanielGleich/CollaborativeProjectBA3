using UnityEngine;

public abstract class UISteamProfileNameDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected UISteamProfile steamProfile;

    void OnEnable()
    {
        steamProfile.OnNameChanged += UpdateNameDisplay;
    }
    void OnDisable()
    {
        steamProfile.OnNameChanged -= UpdateNameDisplay;
    }
    protected abstract void UpdateNameDisplay(string name);
}
