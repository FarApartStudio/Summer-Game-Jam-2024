using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsManager : MonoBehaviour
{
    public event Action<RuntimeStat> OnStatModified;

    [SerializeField] private List<Stat> stats = new List<Stat>();
    [SerializeField] private List<RuntimeStat> runtimeStats = new List<RuntimeStat>();
    [SerializeField] private List<TimedStatusEffect> _timedStatusEffects = new List<TimedStatusEffect>();

    private void Start()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        stats.ForEach(x => AddStat(x.statType, x.baseValue));
    }

    public void AddStat(StatType statType, float baseValue)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statType);
        if (runtimeStat == null)
        {
            runtimeStat = new RuntimeStat(statType, baseValue);
            runtimeStats.Add(runtimeStat);
            OnStatModified?.Invoke(runtimeStat);
        }
    }

    public void AddModifier(StatEffector statEffector)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statEffector.statType);
        if (runtimeStat != null)
        {
            runtimeStat.AddModifier(statEffector);
            OnStatModified?.Invoke(runtimeStat);
        }
    }

    public void RemoveModifier(StatEffector statEffector)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statEffector.statType);
        if (runtimeStat != null)
        {
            runtimeStat.RemoveModifier(statEffector);
            OnStatModified?.Invoke(runtimeStat);
        }
    }

    public void AddTimedStatusEffect(StatEffector statEffector, float duration)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statEffector.statType);
        if (runtimeStat != null)
        {
            TimedStatusEffect timedStatusEffect = _timedStatusEffects.Find(x => !x.IsActive);
            if (timedStatusEffect == null)
            {
                timedStatusEffect = new TimedStatusEffect(this, this, statEffector, duration);
                _timedStatusEffects.Add(timedStatusEffect);
            }
            else
            {
                timedStatusEffect.Activate(this, this, statEffector, duration);
            }
        }
    }

    public float GetStatValue(StatType statType)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statType);
        if (runtimeStat != null)
        {
            return runtimeStat.GetCurrentValue;
        }
        return 0;
    }

    public void SetBaseValue(StatType statType, float value)
    {
        RuntimeStat runtimeStat = runtimeStats.Find(x => x.statType == statType);
        if (runtimeStat != null)
        {
            runtimeStat.baseValue = value;
            OnStatModified?.Invoke(runtimeStat);
        }
        else
        {
            AddStat(statType, value);
        }
    }

    public void ResetStats()
    {
        runtimeStats.ForEach(x =>
        {          
            x.ResetStat();
            OnStatModified?.Invoke(x);
        });

        _timedStatusEffects.ForEach(x => x.RemoveEffect());
        _timedStatusEffects.Clear();
    }

    private void OnDisable()
    {
        ResetStats();
    }
}

[Serializable]
public class RuntimeStat
{
    public StatType statType;
    public float baseValue;
    public List<RuntimeStatModifier> runtimeStatModifiers = new List<RuntimeStatModifier>();
    public float currentValue;

    public float GetCurrentValue { get => currentValue; }
    public List<RuntimeStatModifier> GetRuntimeStatModifiers { get => runtimeStatModifiers; }

    public RuntimeStat (StatType statType, float startValue)
    {
        this.statType = statType;
        currentValue = baseValue = startValue;
    }

    public void AddModifier(StatEffector statEffector)
    {
        float statValue = 0;
        switch (statEffector.modifierCalculationType)
        {
            case ModifierCalculationType.Flat:
                statValue = statEffector.value;
                break;
            case ModifierCalculationType.Percent:
                statValue = StatsModifier.GetValueOfPercentage(baseValue, statEffector.value);
                break;
        }
        runtimeStatModifiers.Add(new RuntimeStatModifier(statEffector, statValue));
        CalculateValue();
    }

    public void RemoveModifier(StatEffector statEffector)
    {
        RuntimeStatModifier statModifier = runtimeStatModifiers.Find(x => x.key == statEffector.key);
        runtimeStatModifiers.Remove(statModifier);
        CalculateValue();
    }

    private void CalculateValue()
    {
        float finalValue = baseValue;
        runtimeStatModifiers.ForEach(x => finalValue += x.modifierOperation == ModifierOperation.Add ? x.value : -x.value);
        currentValue = finalValue;
    }

    public void ResetStat()
    {
        currentValue = baseValue;
        runtimeStatModifiers.Clear();
    }
}

public enum StatType
{
    Health,
    Damage,
    Speed
}