using UnityEngine;

public abstract class UISteamProfileImageDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected UISteamProfile steamProfile;

    void OnEnable()
    {
        steamProfile.OnImageChanged += UpdateImageDisplay;
    }
    void OnDisable()
    {
        steamProfile.OnImageChanged -= UpdateImageDisplay;
    }
    protected abstract void UpdateImageDisplay(Texture image);
}