using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitObserver : MonoBehaviour, IDamageable
{
    [SerializeField] private bool oneTimeUse = false;
    [SerializeField] private UnityEvent OnHit;
    [SerializeField] private UnityEvent <bool> OnHitToggle;
    private bool isHit = false;

    public bool IsHit => isHit;

    public UnityEvent GetOnHit => OnHit;

    public bool Damage(DamageInfo damageInfo)
    {
        if (oneTimeUse && isHit) return false;
        isHit = !isHit ? true : false;
        OnHit.Invoke();
        OnHitToggle.Invoke(isHit);
        return true;
    }
}
