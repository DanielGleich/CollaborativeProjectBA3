using System;
using System.Collections;
using UnityEngine;

public class ShipMovement : MonoBehaviour {
    [Header("References")]
    [SerializeField] private BallastScale ballastScale;
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float rotationAmount = 10f;
    [SerializeField] protected float accelerationTime = 0.25f;
    [SerializeField] private float maxRotationAmount;
    [SerializeField] private float ballastSpeedCost = .5f;

    [Header("Boost Settings")]
    [SerializeField, Min(1)] private float boostMultiplier = 1.5f;
    [SerializeField] private float boostTime = 1f;

    private bool boostActive = false;
    public float CurrentSpeed {get; private set;} = 0;

    void OnValidate()
    {
        if(!ballastScale)
            ballastScale = GetComponent<BallastScale>();
    }
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * Mathf.Max((boostActive? baseSpeed * boostMultiplier : baseSpeed) - ballastScale.TotalWeight * ballastSpeedCost, 0),ForceMode.Acceleration);
        transform.localEulerAngles += Vector3.up * Mathf.Clamp(rotationAmount * ballastScale.WeigthBalance, -maxRotationAmount, maxRotationAmount) * Time.deltaTime;
        Debug.Log($"Current Speed = {rb.linearVelocity.magnitude}");
    }
    public void Boost()
    {
        StopAllCoroutines();
        StartCoroutine(BoostRoutine());
    }
    public IEnumerator BoostRoutine()
    {
        boostActive = true;
        yield return new WaitForSeconds(boostTime);
        boostActive = false;
    }
}