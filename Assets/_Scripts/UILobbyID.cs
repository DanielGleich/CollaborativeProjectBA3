using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILobbyID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyIdText;
    [SerializeField] private Texture2D _linkCursorTexture;
    private Color _defaultColor;
    [SerializeField] private Color _clickedColor;

    private void Start()
    {
        _defaultColor = _lobbyIdText.color;
    }

    public void SetCursorToPointer()
    {
        Cursor.SetCursor(_linkCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetTextColorToClicked()
    {
        _lobbyIdText.color = _clickedColor;
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void ResetTextColor()
    {
        _lobbyIdText.color = _defaultColor;
    }
}
