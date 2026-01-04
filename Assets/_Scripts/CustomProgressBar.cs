using UnityEngine;

public class CustomProgressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform BgTransform;
    [SerializeField] RectTransform ProgressTransform;

    Vector2 MaxBarSize = Vector2.zero;
    void Start()
    {
        MaxBarSize = BgTransform.sizeDelta;
        ProgressTransform.sizeDelta = new Vector2(0, MaxBarSize.y);
    }

    public void SetValue(float val) 
    {
        ProgressTransform.sizeDelta = new Vector2(Mathf.Lerp(0, MaxBarSize.x, val), MaxBarSize.y);
    }
}
