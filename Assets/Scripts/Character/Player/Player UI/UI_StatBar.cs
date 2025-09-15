using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    private RectTransform rectTransform;

    [Header("Bar Options")]
    protected bool scaleBarLenghtWithStats = true;
    public float widthScaleMultiplier = 3f;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(int newValue)
    {
        Console.WriteLine(newValue);
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLenghtWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
            PlayerUIManager.Instance.PlayerUIHudManager.RefreshHUD();
        }
    }
}