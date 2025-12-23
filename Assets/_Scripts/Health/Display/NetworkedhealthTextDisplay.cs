using TMPro;
using UnityEngine;

public class NetworkedhealthTextDisplay : NetworkedHealthDisplay
{
    [Header("UI-Elements")]
    [SerializeField] private TMP_Text healthText;

    [Header("Formating")]
    [SerializeField] private string format = "{0}/{1}";

    void OnValidate()
    {
        if(!healthText)
            healthText = GetComponentInChildren<TMP_Text>();
        if(healthText)
            healthText.SetText(string.Format(format,0,3));
    }
    protected override void UpdateHealth(float prev, float next, bool asServer)
    {
        healthText.SetText(string.Format(format,Mathf.RoundToInt(networkedHealth.CurrentHealth.Value),networkedHealth.MaxHealth));
    }
}