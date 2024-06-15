using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]   
public class AttackInfo 
{
    public enum AttackType { Normal, Special}

    public string attackName;
    public DamageType damageType;
    public AttackType attackType;
    public bool knockBack;
    public int damage;
    public float damageMultiply = 1;
    public Damager damager;
    public UnityEvent OnAttack;
    public Action OnHit;
}

public class AttackInfoManager : MonoBehaviour
{
    public event Action<bool> OnAttack;

    [SerializeField] private AttackInfo[] attackInfos;

    private AttackInfo attackInfo;

    public void SetAllDamage(int amount, bool allowknockBack)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.damage = amount;
            if(!allowknockBack) attackInfo.knockBack = false;
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
            if (attackInfo.attackType == attackType) continue;
            attackInfo.OnHit += action;
        }
    }

    public void RegisterHitEventForOnly(AttackInfo.AttackType attackType,Action action)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            if(attackInfo.attackType != attackType) continue;
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
            if (attackInfo.attackType != attackType || attackInfo.damageType != damageType) continue;
            attackInfo.damage = currentDamage;
            attackInfo.damager.SetUp(owner, currentCriticalDamage, currentCriticalChance);   
        }
    }
    public void SetDamagerInfo(MonoBehaviour owner, int currentDamage,int currentCriticalDamage = 0, float currentCriticalChance = 0)
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.damage = currentDamage;
            attackInfo.damager.SetUp(owner,currentCriticalDamage, currentCriticalChance);
        }
    }

    public void Damage(string attackName)
    {
        attackInfo = GetAttackInfo(attackName);
        if (attackInfo == null) return;

        attackInfo.damager.damageInfo.damageType = attackInfo.damageType;
        attackInfo.damager.damageInfo.damage = attackInfo.damage;
        attackInfo.damager.damageInfo.damageMultiply = attackInfo.damageMultiply;
        attackInfo.damager.damageInfo.knockback = attackInfo.knockBack;
        attackInfo.damager.damageInfo.stun = attackInfo.attackType == AttackInfo.AttackType.Special;
        attackInfo.damager.OnHit = attackInfo.OnHit;
        attackInfo.damager.Attack();
        attackInfo.OnAttack?.Invoke();
        OnAttack?.Invoke(attackInfo.knockBack);
    }

    private void OnDisable()
    {
        foreach (AttackInfo attackInfo in attackInfos)
        {
            attackInfo.OnHit = null;
            attackInfo.damager.OnHit = null;
        }
    }
}
