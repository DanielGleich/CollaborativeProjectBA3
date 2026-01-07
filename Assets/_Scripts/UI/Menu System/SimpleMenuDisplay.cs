using System;

/// <summary>
/// Enables/ disables submenus based on menu state manager
/// </summary>
public class SimpleMenuDisplay : MenuDisplayHandler
{
    protected override void UpdateDisplay()
    {
        Array.ForEach(menuStateManager.SubMenus, m => m.gameObject.SetActive(m == menuStateManager.CurrentSubMenu && menuStateManager.MenuIsActive));
    }
}