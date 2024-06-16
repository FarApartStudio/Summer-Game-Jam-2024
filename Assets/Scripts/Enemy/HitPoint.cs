using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour,IDamageable
{
    public int damageMultiplier = 1;

    public event Action<HitPoint> OnHit;

    IHealth _health;
    Collider _collider;
    DamageInfo damageInfo;

    public int GetDamageMultiplier => damageMultiplier;
    public DamageInfo GetDamageInfo => damageInfo;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void AssignHealthSystem(IHealth health)
    {
        _health = health;
    }

    public void Damage(DamageInfo damageInfo, Vector3 damagePos)
    {
        this.damageInfo = damageInfo;
        damageInfo.damage = (damageInfo.damage * damageMultiplier);
        _health.DealDamage(damageInfo);
        OnHit?.Invoke(this);

        PopUpTextManager.Instance.PopUpAtTextPosition(PopUpTextManager.PopUpType.UI, damagePos, Vector3.zero, damageInfo.damage.ToString(), damageMultiplier == 1 ? Color.yellow : Color.red);
    }

    public bool LastKillShot(int damage)
    {
        if((damage * damageMultiplier) >= _health.GetCurrentHealth)
        {
            return true;
        }

        return false;
    }

    public void ToggleActive (bool active)
    {
        _collider.enabled = active;
    }
}
