using UnityEngine;

/// <summary>
/// Responsible for ensuring that mouse inputs definitely work when a menu is displayed
/// </summary>
public class MenuMouseHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuStateManager menuStateManager;

    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisibility;

    void OnEnable()
    {
        menuStateManager.OnUpdateMenuIsActive += UpdateMenuIsActive;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateMenuIsActive(menuStateManager.MenuIsActive);
    }
    void OnDisable()
    {
        menuStateManager.OnUpdateMenuIsActive -= UpdateMenuIsActive;
    }

    private void UpdateMenuIsActive(bool menuIsActive)
    {
        if(menuIsActive)
        {
            // Get old cursor state
            previousCursorLockMode = Cursor.lockState;
            previousCursorVisibility = Cursor.visible;

            // Update Cursor state
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = previousCursorLockMode;
            Cursor.visible = previousCursorVisibility;
        }
    }
}
