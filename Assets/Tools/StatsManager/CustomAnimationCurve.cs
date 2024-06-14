using System;
using UnityEngine;
using Sirenix.OdinInspector;

public enum CurveType
{
    Custom,
    Linear,
    EaseInOut,
    CustomKeys
}

[Serializable]
public struct CustomCurveKeys
{
    public CustomCurveKey[] keys;
}

[Serializable]
public struct CustomCurveKey
{
    public float time;
    public float value;
}

[Serializable]
public struct CurveValues
{
    public float minLevel;
    public float minValue;
    public float maxLevel;
    public float maxValue;
}

[Serializable]
public class CustomAnimationCurve
{
    [OnValueChanged("UpdateCurveValues")]
    public AnimationCurve animationCurve;

    [OnValueChanged("UpdateCurve")]
    public CurveType curveType;

    [OnValueChanged("UpdateCurve"), HideIf("curveType", CurveType.CustomKeys)]
    public CurveValues curveValues;

    [ShowIf ("curveType", CurveType.CustomKeys), OnValueChanged("UpdateCurveFromCustomKeys"), OnValueChanged("UpdateCurveValues")]
    public CustomCurveKeys customCurveKeys;

    public void UpdateCurveValues()
    {
        if (curveType == CurveType.Custom) return;

        if (animationCurve.keys.Length > 0)
        {
            curveValues.minLevel = Mathf.RoundToInt(animationCurve.keys[0].time);
            curveValues.minValue = Mathf.RoundToInt(animationCurve.keys[0].value);
            curveValues.maxLevel = Mathf.RoundToInt(animationCurve.keys[animationCurve.keys.Length - 1].time);
            curveValues.maxValue = Mathf.RoundToInt(animationCurve.keys[animationCurve.keys.Length - 1].value);
        }
        else
        {
            curveValues.minLevel = 0;
            curveValues.minValue = 0;
            curveValues.maxLevel = 0;
            curveValues.maxValue = 0;
        }

        UpdateCustomKeys();
    }

    public void UpdateCurve()
    {
        switch(curveType)
        {
            case CurveType.Custom:
                break;
            case CurveType.Linear:
                animationCurve = AnimationCurve.Linear(curveValues.minLevel, curveValues.minValue, curveValues.maxLevel, curveValues.maxValue);
                break;
            case CurveType.EaseInOut:
                animationCurve = AnimationCurve.EaseInOut(curveValues.minLevel, curveValues.minValue, curveValues.maxLevel, curveValues.maxValue);
                break;
            case CurveType.CustomKeys:
            UpdateCurveFromCustomKeys();
            break;
        }

        UpdateCurveValues();
    }

    public void UpdateCustomKeys()
    {
        customCurveKeys = new CustomCurveKeys();
        customCurveKeys.keys = new CustomCurveKey[animationCurve.keys.Length];
        for (int i = 0; i < animationCurve.keys.Length; i++)
        {
            customCurveKeys.keys[i] = new CustomCurveKey();
            customCurveKeys.keys[i].time = animationCurve.keys[i].time;
            customCurveKeys.keys[i].value = animationCurve.keys[i].value;
        }      
    }

    public void UpdateCurveFromCustomKeys()
    {
        animationCurve.ClearKeys();
        for (int i = 0; i < customCurveKeys.keys.Length; i++)
        {
            animationCurve.AddKey(customCurveKeys.keys[i].time, customCurveKeys.keys[i].value);
        }

        for (int i = 0; i < animationCurve.keys.Length; i++)
        {
            if (i == 0 || i == animationCurve.keys.Length - 1) continue;
            animationCurve.SmoothTangents(i, 0);
        }
    }

    public int GetCurrentValueInt(int level) { return Mathf.RoundToInt(animationCurve.Evaluate(level)); }
    public float GetCurrentValueFloat(int level) { return animationCurve.Evaluate(level); }

    public int GetTotalValue(int level)
    {
        int totalAmount = 0;
        for (int i = 1; i < level; i++) totalAmount += Mathf.RoundToInt(animationCurve.Evaluate(i));
        return totalAmount;
    }
}