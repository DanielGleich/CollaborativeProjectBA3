using UnityEngine;
using UnityEngine.UI;

public class VsyncToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void OnValidate()
    {
        if (!toggle)
            toggle = GetComponentInChildren<Toggle>();
    }
    private void OnEnable()
    {
        toggle.isOn = QualitySettings.vSyncCount != 0;
        toggle.onValueChanged.AddListener(SetVsync);
    }

    public void SetVsync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }
}