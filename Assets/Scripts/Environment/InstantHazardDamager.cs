using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverlapChecker))]
public class InstantHazardDamager : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool knockback;

    private OverlapChecker overlapChecker;

    private void Awake()
    {
        overlapChecker = GetComponent<OverlapChecker>();
    }

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;
    }

    private void OnOverlap(int arg1, Collider[] arg2)
    {
        for (int i = 0; i < arg1; i++)
        {
            if (arg2[i].TryGetComponent(out IHealth damageable))
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = damage;
                damageInfo.knockback = knockback;
                damageInfo.hitDirection = (arg2[i].transform.position - transform.position).normalized;
                damageInfo.damagePosition = transform.position;
                damageable.DealDamage(damageInfo);
            }
        }
    }

    public void DealDamage()
    {
        overlapChecker.DetectOverlap();
    }
}
