using UnityEngine;

/// <summary>
/// Lets you open the webbrowser via button click
/// </summary>
public class UrlOpener : MonoBehaviour
{
    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}
