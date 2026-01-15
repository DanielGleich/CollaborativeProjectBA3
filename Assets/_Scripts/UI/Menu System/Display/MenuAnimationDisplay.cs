using System;
using UnityEngine;

/// <summary>
/// Triggers animations based on transitions between sub menus (A transition animation between every single subMenu is required)
/// </summary>
public class MenuAnimationDisplay : MenuDisplayHandler
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Animation Transitions")]
    [SerializeField] private SubMenuTransition[] transitions;
    [SerializeField] private string openAnimationTrigger;
    [SerializeField] private string closeAnimationTrigger;

    [ContextMenu("Generate all needed transitions")]
    private void GenerateAllNeededTransitions()
    {
        // ToDo: implement some day (I don't think we will use this component in this project)
        Debug.LogWarning("This method is not implemented yet");
    }
    protected override void UpdateDisplay()
    {
        SubMenuTransition currentTransition = Array.Find(transitions, t => t.PreviousSubMenu == previousSubMenu && t.NextSubMenu == currentSubMenu);
        if (currentTransition == null)
        {
            Debug.LogError("No transition set");
            return;
        }
        animator.SetTrigger(currentTransition.AnimationTriggerName);
    }
    
    [Serializable]
    private class SubMenuTransition
    {
        public SubMenuTransition(SubMenu prev, SubMenu next)
        {
            PreviousSubMenu = prev;
            NextSubMenu = next;
        }
        [field: SerializeField] public SubMenu PreviousSubMenu;
        [field: SerializeField] public SubMenu NextSubMenu;
        [field: SerializeField] public string AnimationTriggerName;
    }
}