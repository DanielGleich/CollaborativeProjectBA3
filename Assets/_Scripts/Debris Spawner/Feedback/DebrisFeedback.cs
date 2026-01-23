using FMODUnity;
using UnityEngine;

public class DebrisFeedback : MonoBehaviour {
    [Header("References"), SerializeField] private Debris debris;
    [SerializeField] private GameObject destoyedPrefab;
    [SerializeField] private EventReference destroctionSFX;

    void OnValidate()
    {
        if(!debris)
            debris = GetComponent<Debris>();
    }
    void OnEnable()
    {
        debris.OnDestroy += DestoryFeedback;
    }
    void OnDisable()
    {
        debris.OnDestroy -= DestoryFeedback;
    }
    private void DestoryFeedback()
    {
        if(destoyedPrefab)
            Instantiate(destoyedPrefab, debris.transform.position, debris.transform.rotation);
        if(string.IsNullOrEmpty(destroctionSFX.ToString()))
            RuntimeManager.PlayOneShot(destroctionSFX, debris.transform.position);
    }
}