using System;
using UnityEngine;

/// <summary>
/// Makes a game object with a collider affect the ballast scale
/// </summary>
public class Ballast : MonoBehaviour
{
    [SerializeField] public float Weight;
    public event Action<Ballast> OnDestroyBallast;
    void OnDestroy()
    {
        OnDestroyBallast?.Invoke(this);
    }
}
