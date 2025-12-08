using TMPro;
using UnityEngine;

public class HealthTextDisplay : HealthDisplay
{
    [Header("UI-Elements")]
    [SerializeField] private TMP_Text healthText;

    [Header("Settings")]
    [SerializeField] private string format = "{0}/{1}";
    void OnValidate()
    {
        if(!healthText)
            healthText = GetComponentInChildren<TMP_Text>();
        if(healthText)
            healthText.SetText(string.Format(format,0,3));
    }
    protected override void UpdateHealthDisplay(float currentHealth)
    {
        healthText.SetText(string.Format(format,currentHealth,health.MaxHealth));
    }
}