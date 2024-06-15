using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GroupSliderFill : MonoBehaviour
{
    [Range(0.0f, 1.0f)][SerializeField] private float _value = 0f;
    [SerializeField] private List<Slider> _bars;

    private void OnValidate()
    {
        LoadFills();
        UpdateValue();
    }

    public void AddSlider(Slider slider)
    {
        _bars.Add(slider);
    }

    public void Clear()
    {
        _bars.Clear();
    }

    public void LoadFills()
    {
        _bars = GetComponentsInChildren<Slider>().ToList();
        foreach (Slider fill in _bars)
        {
            fill.transition = Selectable.Transition.None;
            fill.interactable = false;
        }
    }

    private void UpdateValue()
    {
        int numFills = _bars.Count;
        if (numFills <= 0)
        {
            //Debug.LogError("No Image fills assigned to the GroupSlider!");
            return;
        }

        float fillWidth = 1f / numFills;
        for (int i = 0; i < numFills; i++)
        {
            _bars[i].value = Mathf.Clamp01(_value - i * fillWidth) / fillWidth;
            _bars[i].enabled = _value >= i * fillWidth;
        }
    }

    public void SetValue(float value)
    {
        _value = value;
        UpdateValue();
    }

    public void SetValue(float value, float maxValue)
    {
        _value = value / maxValue;
        UpdateValue();
    }

    public void ToggleVisbility(bool visible)
    {
        foreach (Slider fill in _bars)
        {
            fill.enabled = visible;
        }
    }
}
