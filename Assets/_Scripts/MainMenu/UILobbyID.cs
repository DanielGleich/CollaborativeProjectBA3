using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILobbyID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyIdText;
    [SerializeField] private Texture2D linkCursorTexture;
    private Color defaultColor;
    [SerializeField] private Color clickedColor;

    private void Start()
    {
        defaultColor = lobbyIdText.color;
    }

    public void SetCursorToPointer()
    {
        Cursor.SetCursor(linkCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetTextColorToClicked()
    {
        lobbyIdText.color = clickedColor;
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ResetTextColor()
    {
        lobbyIdText.color = defaultColor;
    }
}
