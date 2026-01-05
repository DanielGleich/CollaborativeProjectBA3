using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Start Game"))
        {
            gameManager.StartGame();
        }
    }
}
