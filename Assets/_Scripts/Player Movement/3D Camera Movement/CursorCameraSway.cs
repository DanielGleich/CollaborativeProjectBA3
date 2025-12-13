using UnityEngine;
using UnityEngine.InputSystem;

public class CursorCameraSway : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveStrength = 0.025f;
    [SerializeField] private float rotationStrength = 0.025f;
    private Vector3 startPosition;
    private Vector3 startRotation;

    void Awake()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localEulerAngles;
    }
    void Update()
    {
        // mousePosRelativeToCenter has a value between -1 and 1
        Vector2 mousePosRelativeToCenter = (Mouse.current.position.ReadValue() - new Vector2(Screen.width, Screen.height) / 2) / Mathf.Max(Screen.width, Screen.height) * 2;
        // Move Object
        transform.localPosition = startPosition + transform.right * mousePosRelativeToCenter.x * moveStrength + transform.up * mousePosRelativeToCenter.y * moveStrength;
        // Rotate Object
        transform.localEulerAngles = startRotation + new Vector3(-mousePosRelativeToCenter.y * rotationStrength, mousePosRelativeToCenter.x * rotationStrength, 0);
    }
}