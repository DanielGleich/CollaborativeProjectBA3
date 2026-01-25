using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Switches between multiple Cinemachine cameras by changing the priority 
/// </summary>
public class CinemachineViewSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The game always starts at the first view [0]")] private CinemachineCamera[] availableViews;

    [Header("Settings")]
    [SerializeField] private SwitchType switchType;

    private int currentIndex = 0;

    [ContextMenu("Get all avialable views in children")]
    private void GetViewsInChildren()
    {
        availableViews = GetComponentsInChildren<CinemachineCamera>(true);
    }
    private void Awake() => SelectAvailableView(0);
    public void SelectAvailableView(int index)
    {
        if (index >= availableViews.Length)
        {
            Debug.LogError("Out of range");
            return;
        }
        currentIndex = index;
        SelectAvailableView(availableViews[index]);
    }
    public void SelectAvailableView(CinemachineCamera selectedView)
    {
        if (!availableViews.Contains(selectedView))
        {
            Debug.LogError("The selected Cinemachine Camera is not available");
            return;
        }
        Array.ForEach(availableViews, x => x.Priority = x == selectedView ? 10 : -1);
    }
    public void SwitchView()
    {
        switch (switchType)
        {
            case SwitchType.Up:
                currentIndex = (currentIndex + 1) % availableViews.Length;
                break;

            case SwitchType.Down:
                currentIndex = (currentIndex - 1) % availableViews.Length;
                break;

            case SwitchType.Random:
                currentIndex = UnityEngine.Random.Range(0, availableViews.Length);
                break;
        }
        SelectAvailableView(currentIndex);
    }

    private enum SwitchType
    {
        Up, Down, Random
    }
}