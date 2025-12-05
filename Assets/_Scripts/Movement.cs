using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;
    Vector3 _moveTo = Vector3.zero;
    private void FixedUpdate()
    {
        if (_moveTo == Vector3.zero) return;

        Vector3 worldDir = transform.TransformDirection(_moveTo.normalized);
        transform.position += worldDir * _moveSpeed * Time.deltaTime;
    }

    public void MoveForward()
    {
        _moveTo.z = 1;
    }

    public void MoveBackward()
    {
        _moveTo.z = -1;
    }

    public void MoveLeft()
    {
        _moveTo.x = -1;
    }

    public void MoveRight()
    { 
        _moveTo.x = 1;
    }

    public void SetMovement(Vector3 movement)
    { 
        _moveTo = movement;
    }

    public void RotateToTransform(Transform t)
    { 
        
    }

    public void RotateLeft()
    { 
    
    }

    public void RotateRight()
    { 
        
    }

    public void Rotate(Vector3 dir)
    {
        transform.Rotate(dir);
    }
}
