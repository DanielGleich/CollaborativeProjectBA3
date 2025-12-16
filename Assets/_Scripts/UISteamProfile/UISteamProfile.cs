using Steamworks;
using System;
using UnityEngine;

public class UISteamProfile : MonoBehaviour
{
    public event Action<Texture> OnImageChanged;
    public event Action<string> OnNameChanged;
    public event Action OnProfileEmpty;
    public event Action OnProfileFilled;

    private CSteamID currentSteamId;
    public CSteamID CurrentSteamId 
    {get => currentSteamId;
        set 
        {
            currentSteamId = value;
            if (value == CSteamID.Nil)
            {
                OnProfileEmpty?.Invoke();
                return;
            }
            OnProfileFilled?.Invoke();
            OnNameChanged?.Invoke(SteamFriends.GetFriendPersonaName(currentSteamId));
            int imageId = SteamFriends.GetLargeFriendAvatar(currentSteamId);
            if (imageId == -1) return;
            OnImageChanged?.Invoke(GetSteamImageAsTexture(imageId));
        }
    }

    private void Awake()
    {
        currentSteamId = CSteamID.Nil;
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
            if (isValid)
            {
                int rowSize = (int)(width * 4);
                byte[] flipped = new byte[image.Length];

                for (int y = 0; y < height; y++)
                {
                    int srcRow = y * rowSize;
                    int dstRow = (int)((height - 1 - y) * rowSize);
                    System.Buffer.BlockCopy(image, srcRow, flipped, dstRow, rowSize);
                }

                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(flipped);
                texture.Apply();
            }
        }

        return texture;
    }
}
