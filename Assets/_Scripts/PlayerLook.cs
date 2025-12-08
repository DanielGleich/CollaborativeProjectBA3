using Unity.Cinemachine;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Movement _playerMovement;

    [Header("Settings")]
    [SerializeField] private float _sensitivity = 1.0f;
    [SerializeField] private float _minPitch = -80f;
    [SerializeField] private float _maxPitch = 80f;

    private float _pitch;

    public void ApplyLook(Vector2 lookDelta)
    {
        float mouseX = lookDelta.x * _sensitivity;
        float mouseY = lookDelta.y * _sensitivity;


        _playerMovement.Rotate(Vector3.up * lookDelta.x * _sensitivity);

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        _cameraRoot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
    }

    public void SetLocalPlayerCamera()
    {
        _cameraRoot.GetComponent<CinemachineCamera>().Prioritize();
    }
}
