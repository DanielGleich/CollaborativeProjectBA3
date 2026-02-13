using UnityEditor;
using UnityEngine;

/*<summary>
 * Adds a button to the GameManager editor window
 * </summary>*/

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

        if (GUILayout.Button("End Game"))
        {
            gameManager.DebugGameEnd();
        }
    }
}
