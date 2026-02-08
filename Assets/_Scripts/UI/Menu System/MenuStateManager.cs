using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps track of current state of the menu and all of the submenus (usable in many different settings like main menu, pause menu, level select screen and much more!)
/// </summary>
public class MenuStateManager : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] public SubMenu StartMenu {get; private set;}
    [field: SerializeField] public SubMenu[] SubMenus {get; private set;}

    [Header("Settings")] 
    [SerializeField] private bool resetOnDisableMenu = true;
    [SerializeField] private bool startActive;

    private SubMenu currentSubMenu;
    public event Action<SubMenu> OnUpdateCurrentSubMenu;
    public SubMenu CurrentSubMenu
    {
        get => currentSubMenu;
        private set
        {
            if(value == currentSubMenu)
                return;
            currentSubMenu = value;
            Array.ForEach(SubMenus, s => s.IsActive = s == value);
            OnUpdateCurrentSubMenu?.Invoke(value);
        }
    }
    private bool menuIsActive;
    public event Action<bool> OnUpdateMenuIsActive;
    public bool MenuIsActive 
    {
        get => menuIsActive;
        private set
        {
            if(value == menuIsActive)
                return;
            menuIsActive = value;
            OnUpdateMenuIsActive?.Invoke(value);
        }
    }

    [ContextMenu("Get all sub-menus in children")]
    private void GetSubMenusInChildren() => SubMenus = GetComponentsInChildren<SubMenu>(true);

    void OnValidate()
    {
        if(StartMenu && !SubMenus.Contains(StartMenu))
            Debug.LogWarning("The start menu has to be present in the sub menus array");
    }
    private void Awake()
    {
        CurrentSubMenu = StartMenu;
        if(startActive)
            SetMenuActive(true);
    }
    public void SetCurrentSubMenu(SubMenu subMenu)
    {
        if(!SubMenus.Contains(subMenu))
        {
            Debug.LogError($"The sub menu {subMenu.gameObject} is not part of the sub menus array");
            return;
        }
        CurrentSubMenu = subMenu;
    }
    [ContextMenu("Toggle Menu Active")]
    public void ToggleMenuActive() => SetMenuActive(!MenuIsActive);
    public void ResetActiveMenu() => CurrentSubMenu = StartMenu;
    public void SetMenuActive(bool active)
    {
        MenuIsActive = active;
        if(!active && resetOnDisableMenu)
            ResetActiveMenu();
    }
}