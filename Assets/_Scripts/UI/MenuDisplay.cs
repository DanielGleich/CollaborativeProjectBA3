using UnityEngine;

/// <summary>
/// Updates the menu display based on the menu manager
/// </summary>
public abstract class MenuDisplayHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected MenuStateManager menuStateManager;

    [Header("Settings")]
    [SerializeField, Tooltip("Relevant for transitions. Keep active if ")] private bool clearOnDeactivateMenu = true;

    protected SubMenu currentSubMenu;
    protected SubMenu previousSubMenu;

    void OnEnable()
    {
        menuStateManager.OnUpdateCurrentSubMenu += UpdateCurrentSubMenu;
        menuStateManager.OnUpdateMenuIsActive += UpdateMenuActive;
        UpdateMenuActive(menuStateManager.MenuIsActive);
    }
    void OnDisable()
    {
        menuStateManager.OnUpdateCurrentSubMenu -= UpdateCurrentSubMenu;
        menuStateManager.OnUpdateMenuIsActive -= UpdateMenuActive;
    }

    private void UpdateMenuActive(bool isActive)
    {
        UpdateCurrentSubMenu(menuStateManager.CurrentSubMenu);
    }
    private void UpdateCurrentSubMenu(SubMenu subMenu)
    {
        if (menuStateManager.MenuIsActive && subMenu != currentSubMenu)
        {
            previousSubMenu = currentSubMenu;
            currentSubMenu = subMenu;
        }
        else if (clearOnDeactivateMenu)
        {
            previousSubMenu = null;
            currentSubMenu = null;
        }
        UpdateDisplay();
    }
    protected abstract void UpdateDisplay();
}