using System;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Set priorities for cinemachine cameras based on the current sub menu
/// </summary>
public class CinemachineMenuSwitch : MenuDisplayHandler
{
    [SerializeField] private SubMenuCinemachineCamera[] subMenuCinemachineCameras;

    [Header("Settings")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inActivePriority = -10;

    protected override void UpdateDisplay()
    {
        // Check if the submenu has a cinemachine camera assigned
        if(Array.Find(subMenuCinemachineCameras, s => s.SubMenu == currentSubMenu) == null)
        {
            Debug.LogWarning("No cinemachine is assigned to this sub menu. no transition accures");
            return;
        }
        Array.ForEach(subMenuCinemachineCameras, s => 
            s.CinemachineCamera.Priority = s.SubMenu == currentSubMenu && menuStateManager.MenuIsActive? activePriority : inActivePriority);
    }
    [ContextMenu("Create SubMenuCinemachine Camera for every SubMenu in MenustateManager")]
    private void CreateSubMenuCinemachineCameras()
    {
        if(!menuStateManager)
            return;
        subMenuCinemachineCameras = new SubMenuCinemachineCamera[menuStateManager.SubMenus.Length];
        for(int i = 0; i < menuStateManager.SubMenus.Length; i++)
            subMenuCinemachineCameras[i] = new SubMenuCinemachineCamera(menuStateManager.SubMenus[i]);
    }

    [Serializable]
    public class SubMenuCinemachineCamera
    {
        public SubMenuCinemachineCamera(SubMenu subMenu)
        {
            this.SubMenu = subMenu;
        }
        [field: SerializeField] public SubMenu SubMenu;
        [field: SerializeField] public CinemachineCamera CinemachineCamera;
    }
}