using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Switches between multiple Cinemachine cameras by changing the priority 
/// The "Switch Interaction" component is ideal for manually switching between views
/// </summary>
public class CinemachineViewSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera[] availableViews;

    void OnValidate()
    {
        if (availableViews == null)
            availableViews = GetComponentsInChildren<CinemachineCamera>(true);
    }
    public void SelectAvailableView(int index)
    {
        if (index <= availableViews.Length)
        {
            Debug.LogError("Out of range");
            return;
        }
        SelectAvailableView(availableViews[index]);
    }
    public void SelectAvailableView(CinemachineCamera selectedView)
    {
        if (!availableViews.Contains(selectedView))
        {
            Debug.LogError("The selected Cinemachine Camera is not available");
            return;
        }
        Array.ForEach(availableViews, x => x.Priority = 0);
        selectedView.Priority = 1;
    }
}