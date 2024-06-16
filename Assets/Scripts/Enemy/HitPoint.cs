using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour,IDamageable
{
    public int damageMultiplier = 1;

    public delegate void OnHitEventHandler(DamageInfo damageInfo, Vector3 damagePos);
    public event OnHitEventHandler OnHit;

    IHealth _health;
    Collider _collider;

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
        damageInfo.damage = (damageInfo.damage * damageMultiplier);
        _health.DealDamage(damageInfo);
        OnHit?.Invoke(damageInfo, damagePos);
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
