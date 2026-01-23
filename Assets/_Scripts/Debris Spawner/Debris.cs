using System;
using UnityEngine;

[RequireComponent(typeof(Ballast)), RequireComponent(typeof(Grabable3d)), RequireComponent(typeof(Rigidbody))]
public class Debris : MonoBehaviour {
    [SerializeField] private GameObject FracturedDebris;
    public event Action OnDestroy;
    public void DestoyDebris()
    {
        OnDestroy?.Invoke();
        Destroy(gameObject);
    }
}