using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollisionFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject vfxPrefab;

    [Header("Settings")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool UseTriggerEvents;
    [SerializeField] bool UseCollisionEvents;
    [SerializeField] float spawnCooldown;

    List<GameObject> gameObjectsOnCooldown = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (UseTriggerEvents == false) return;
        if (CanParticleSpawn(other.gameObject) == false) return;
        SpawnParticle(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (UseTriggerEvents == false) return;
        if (CanParticleSpawn(other.gameObject) == false) return;
        SpawnParticle(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (UseCollisionEvents == false) return;
        if (CanParticleSpawn(collision.gameObject) == false) return;
        SpawnParticle(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (UseCollisionEvents == false) return;
        if (CanParticleSpawn(collision.gameObject) == false) return;
        SpawnParticle(collision);
    }

    private bool CanParticleSpawn(GameObject obj)
    {
        if ((layerMask & (1 << obj.layer)) == 0) 
            return false;

        if (gameObjectsOnCooldown.Contains(obj)) 
            return false;

        if (obj.transform.root == gameObject.transform.root) 
            return false;

        return true;
    }

    private void SpawnParticle(Collision col)
    {
        ContactPoint contact = col.contacts[0];
        Instantiate(vfxPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        gameObjectsOnCooldown.Add(col.gameObject);
        StartCoroutine(Cooldown(col.gameObject));
    }

    private void SpawnParticle(Collider col)
    {
        Vector3 spawnPos = col.bounds.center;
        Vector3 thisCenter = GetComponent<Collider>().bounds.center;
        spawnPos = (thisCenter + col.bounds.center) * 0.5f;

        Instantiate(vfxPrefab, spawnPos, Quaternion.identity);
        gameObjectsOnCooldown.Add(col.gameObject);
        StartCoroutine(Cooldown(col.gameObject));
    }


    IEnumerator Cooldown(GameObject obj)
    { 
        yield return new WaitForSeconds(spawnCooldown);
        if (gameObjectsOnCooldown.Contains(obj))
            gameObjectsOnCooldown.Remove(obj);
    }
}
