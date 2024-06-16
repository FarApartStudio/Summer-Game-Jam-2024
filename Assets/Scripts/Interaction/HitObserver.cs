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

    public void Damage(DamageInfo damageInfo, Vector3 damagePosition)
    {
        if (oneTimeUse && isHit) return;

        OnHit.Invoke();
        isHit = !isHit ? true : false;
        OnHitToggle.Invoke(isHit);

    }
}
