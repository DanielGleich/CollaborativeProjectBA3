using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyProfile : MonoBehaviour
{
    [SerializeField] RawImage _profilePicture;
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] GameObject _emptyProfile;
    public CSteamID currentSteamId {get; private set;}

    private void Start()
    {
        currentSteamId = CSteamID.Nil;
    }

    public void SetSteamId(CSteamID playerId)
    {
        currentSteamId = playerId;
        if (playerId == CSteamID.Nil)
        { 
            _emptyProfile.SetActive(true);
            return;
        }
        _emptyProfile.SetActive(false);
        _playerName.text = SteamFriends.GetFriendPersonaName(playerId);
        int imageId = SteamFriends.GetLargeFriendAvatar(playerId);

        if (imageId == -1) return;

        _profilePicture.texture = GetSteamImageAsTexture(imageId);
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
