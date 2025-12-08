using UnityEngine;
using UnityEngine.UI;

public class GenericHealthbarDisplay : HealthDisplay
{
    [Header("UI-Elements")]
    [SerializeField] private Image fillSprite;
    protected override void UpdateHealthDisplay(float currentHealth)
    {
        fillSprite.fillAmount = currentHealth / health.MaxHealth;
    }
}