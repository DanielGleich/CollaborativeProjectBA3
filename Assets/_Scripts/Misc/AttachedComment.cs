using UnityEngine;

/// <summary>
/// A comment you can attach to a Gameobject. Meant for communication within the game engine
/// </summary>
public class AttachedComment : MonoBehaviour {
    [SerializeField, TextArea(3, 50)] private string comment;
}