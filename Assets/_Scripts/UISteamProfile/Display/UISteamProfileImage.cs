using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISteamProfileImage : UISteamProfileImageDisplay
{
    [SerializeField] private RawImage profileImage;

    protected override void UpdateImageDisplay(Texture image)
    {
        profileImage.texture = image;
    }
}
