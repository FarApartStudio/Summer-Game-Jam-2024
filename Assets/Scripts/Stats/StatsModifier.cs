using System;
using System.Collections;
using UnityEngine;

public static class StatsModifier
{
    public static float ModifyValueByPercentage(float baseValue, float currentValue, float percentageAmount, ModifierOperation modifierOperation)
    {
        float percentageValue = (percentageAmount / 100) * baseValue;
        return modifierOperation == ModifierOperation.Add ? currentValue + percentageValue : currentValue - percentageValue;
    }

    public static float ModifyValue(float currentValue, float amount, ModifierOperation modifierOperation)
    {
        return modifierOperation == ModifierOperation.Add ? currentValue + amount : currentValue - amount;
    }

    public static float GetPercentage(float baseValue, float percentageAmount)
    {
        return (percentageAmount / 100) * baseValue;
    }
    
    public static float GetValueOfPercentage(float baseValue, float percent)
    {
        return (baseValue / 100) * percent;
    }
}

[Serializable]
public class TimedStatusEffect
{
    public event Action OnEffectEnd;

    private float _duration;
    [SerializeField] private bool _isActive;
    private StatsManager _statsManager;
    [SerializeField] private StatEffector _statEffector;
    private Coroutine _effectCoroutine;
    private MonoBehaviour _instigator;

    public bool IsActive => _isActive;

    public TimedStatusEffect(MonoBehaviour instigator, StatsManager statsManager, StatEffector statEffector, float duration)
    {
        Activate(instigator, statsManager, statEffector, duration);
    }

    public void Activate(MonoBehaviour instigator, StatsManager statsManager, StatEffector statEffector, float duration)
    {
        _duration = duration;
        _isActive = true;
        _statsManager = statsManager;
        _statEffector = statEffector;
        _instigator = instigator;
        _statsManager.AddModifier(_statEffector);
        _effectCoroutine = instigator.StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        yield return new WaitForSeconds(_duration);
        _isActive = false;
        if (!_instigator.gameObject.activeInHierarchy)
            yield break;
        _statsManager.RemoveModifier(_statEffector);
        OnEffectEnd?.Invoke();
    }

    public void RemoveEffect()
    {
        if (!_isActive)
            return;
        _instigator.StopCoroutine(_effectCoroutine);
        _statsManager.RemoveModifier(_statEffector);
        _isActive = false;
    }
}

[Serializable]
public struct Stat
{
    public StatType statType;
    public float baseValue;

    public Stat(StatType statType, float baseValue)
    {
        this.statType = statType;
        this.baseValue = baseValue;
    }
}

[Serializable]
public struct StatEffector
{
    public string key;
    public StatType statType;
    public ModifierCalculationType modifierCalculationType;
    public ModifierOperation modifierOperation;
    public float value;

    public StatEffector(string key, StatType statType, ModifierCalculationType modifierCalculationType, ModifierOperation modifierOperation, float value)
    {
        this.key = key;
        this.statType = statType;
        this.modifierCalculationType = modifierCalculationType;
        this.modifierOperation = modifierOperation;
        this.value = value;
    }
}

[Serializable]
public class RuntimeStatModifier
{
    public string key;
    public ModifierCalculationType modifierType;
    public ModifierOperation modifierOperation;
    public float value;

    public RuntimeStatModifier(StatEffector statEffector, float value)
    {
        this.key = statEffector.key;
        this.modifierType = statEffector.modifierCalculationType;
        this.modifierOperation = statEffector.modifierOperation;
        this.value = value;
    }
}

[Serializable]
public enum ModifierCalculationType
{
    Flat,
    Percent
}

public enum ModifierOperation
{
    Add,
    Subtract,
}

[Serializable]
public class KeyValue
{
    public string key;
    public float Value;

    public float GetValue()
    {
        return Value;
    }
}

[Serializable]
public struct RangeValue
{
    public float min;
    public float max;

    public RangeValue(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}