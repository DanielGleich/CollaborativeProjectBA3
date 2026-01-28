using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AdaptivMenuMusicFeedback : MonoBehaviour {

    [Header("References")]
    [SerializeField] private MenuStateManager menuStateManager;
    [SerializeField] private EventReference musicTrack;

    [Header("Settings")]
    [SerializeField] private SubMenuMusicPhase[] subMenuMusicPhases;
    [SerializeField] private string paramterName;
    [SerializeField, Tooltip("Necessary in case you want to play the music locally from a radio")] private bool attachToGameObject;

    private EventInstance eventInstance;

    void OnEnable()
    {
        menuStateManager.OnUpdateCurrentSubMenu += UpdateCurrentSubMenu;
        menuStateManager.OnUpdateMenuIsActive += UpdateMenuActive;
        UpdateMenuActive(menuStateManager.MenuIsActive);
        UpdateCurrentSubMenu(menuStateManager.CurrentSubMenu);
        eventInstance = RuntimeManager.CreateInstance(musicTrack);
        if(attachToGameObject)
            RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
    }
    void OnDisable()
    {
        menuStateManager.OnUpdateCurrentSubMenu -= UpdateCurrentSubMenu;
        menuStateManager.OnUpdateMenuIsActive -= UpdateMenuActive;
        eventInstance.release();
    }

    private void UpdateMenuActive(bool isActive)
    {
        if(isActive)
            eventInstance.start();
        else
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void UpdateCurrentSubMenu(SubMenu menu)
    {
        SubMenuMusicPhase subMenuMusicPhase = Array.Find(subMenuMusicPhases, s => s.SubMenu == menu);

        if(subMenuMusicPhase != null && menuStateManager.MenuIsActive)
            eventInstance.setParameterByName(paramterName,subMenuMusicPhase.PhaseValue);
    }

    [ContextMenu("Create SubMenuMusicPhase for every SubMenu in MenuStateManager <i>(Warning: will destroy old values)</i>")]
    private void CreateSubMenuMusicPhases()
    {
        if(!menuStateManager)
            return;
        subMenuMusicPhases = new SubMenuMusicPhase[menuStateManager.SubMenus.Length];
        for(int i = 0; i < menuStateManager.SubMenus.Length; i++)
            subMenuMusicPhases[i] = new SubMenuMusicPhase(menuStateManager.SubMenus[i]);
    }

    [Serializable] public class SubMenuMusicPhase
    {
        public SubMenuMusicPhase(SubMenu subMenu)
        {
            this.SubMenu = subMenu;
        }
        [field: SerializeField] public SubMenu SubMenu{get; private set;}
        [field: SerializeField] public int PhaseValue{get; private set;}
    }
}