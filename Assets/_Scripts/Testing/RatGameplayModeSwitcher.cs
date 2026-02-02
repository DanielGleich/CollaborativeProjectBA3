using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates & deactivated GameObjects based on gameplay mode. Mostly meant to be used in Editor
/// </summary>
public class RatGameplayModeSwitcher : MonoBehaviour {
    public enum GameplayMode
    {
        Cubes, 
        Fork
    }
    
    [Serializable]
    public class GameplayModeGameObjects
    {
        public GameplayModeGameObjects(GameplayMode gameplayMode)
        {
            GameplayMode = gameplayMode;
        }
        [field: SerializeField] public GameplayMode GameplayMode{get; private set;}
        [field: SerializeField] public List<GameObject> GameObjects{get; private set;} = new();
    }

    [Header("Settings")]
    [SerializeField] private GameplayMode gameplayMode;
    [SerializeField] private bool useDebris;

    [SerializeField] private List<GameplayModeGameObjects> gameplayModeGameObjects = new();
    [SerializeField] private GameObject[] debrisSystemObjects;

    void OnValidate()
    {
        UpdateGameplayMode(gameplayMode);
    }
    [ContextMenu("Update Gameplay Mode")]
    private void UpdateGameplayMode() => UpdateGameplayMode(gameplayMode);
    private void UpdateGameplayMode(GameplayMode gameplayMode)
    {
        gameplayModeGameObjects.ForEach(m => m.GameObjects.ForEach(g => g.SetActive(m.GameplayMode == gameplayMode)));
        if(debrisSystemObjects != null)
            Array.ForEach(debrisSystemObjects, d => d.SetActive(useDebris));
    }
    public void SetGameplayMode(GameplayMode gameplayMode)
    {
        this.gameplayMode = gameplayMode;
        UpdateGameplayMode(gameplayMode);
    }
}