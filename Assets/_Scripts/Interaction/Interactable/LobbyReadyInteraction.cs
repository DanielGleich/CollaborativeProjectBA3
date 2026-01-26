using System;
using UnityEngine;

public class LobbyReadyInteraction : Interactable
{
    public static event Action OnLobbyReadyInteraction;
    
    public override void Interact()
    {
        OnLobbyReadyInteraction?.Invoke();
    }
}
