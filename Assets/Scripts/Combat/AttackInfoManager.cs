using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]   
public class AttackInfo 
{
    public enum AttackType { Normal, Special}

    public string attackName;
    public float damageMulti = 1;
    public DamageInfo damageInfo;
    public Damager damager;
    public UnityEvent OnAttack;
    public Action OnHit;
}

public class AttackInfoManager : MonoBehaviour
{
    public event Action<AttackInfo> OnAttack;

    [SerializeField] private AttackInfo[] attackInfos;

    private AttackInfo attackInfo;

    public void SetAllDamage(int amount, bool allowknockBack)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.damageInfo.damage = amount;
            if(!allowknockBack) attackInfo.damageInfo.knockback = false;
        }
    }

    public AttackInfo GetAttackInfo(string attackName)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            if (attackInfo.attackName == attackName) return attackInfo;
        }

        Debug.LogError("AttackInfo not found :" +  attackName);
        return null;
    }

    public void RegisterHitEvent(Action action)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.OnHit += action;
        }
    }

    public void RegisterHitEventExcept(AttackInfo.AttackType attackType, Action action)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            if (attackInfo.damageInfo.attackType == attackType) continue;
            attackInfo.OnHit += action;
        }
    }

    public void RegisterHitEventForOnly(AttackInfo.AttackType attackType,Action action)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            if(attackInfo.damageInfo.attackType != attackType) continue;
            attackInfo.OnHit += action;
        }
    }

    public void UnRegisterHitEvent(Action action)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.OnHit -= action;
        }
    }

    public T GetDamager<T>(string attackName) where T : Damager
    {
        return GetAttackInfo(attackName).damager as T;
    }

    public void SetDamagerInfo(AttackInfo.AttackType attackType, DamageType damageType, MonoBehaviour owner, int currentDamage, int currentCriticalDamage = 0, float currentCriticalChance = 0)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            if (attackInfo.damageInfo.attackType != attackType || attackInfo.damageInfo.damageType != damageType) continue;
            attackInfo.damageInfo.damage = currentDamage;
            attackInfo.damager.SetUp(owner, currentCriticalDamage, currentCriticalChance);   
        }
    }
    public void SetDamagerInfo(MonoBehaviour owner, int currentDamage,int currentCriticalDamage = 0, float currentCriticalChance = 0)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.damageInfo.damage = currentDamage;
            attackInfo.damager.SetUp(owner,currentCriticalDamage, currentCriticalChance);
        }
    }

    public void Damage(string attackName)
    {
        attackInfo = GetAttackInfo(attackName);
        if (attackInfo == null) return;

        DamageInfo damageInfo = attackInfo.damageInfo;
        damageInfo.damage = (int)(damageInfo.damage * attackInfo.damageMulti);

        attackInfo.damager.SetDamageInfo(damageInfo);
        attackInfo.damager.OnHit = attackInfo.OnHit;
        attackInfo.damager.Attack();
        attackInfo.OnAttack?.Invoke();
        OnAttack?.Invoke(attackInfo);
    }
}
