using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamager : Damager
{
    [SerializeField] private OverlapChecker overlapChecker;

    private void Awake()
    {
        overlapChecker.OnDetect += OnDetect;
    }

    private void OnDetect(int count, Collider[] arg2)
    {
        for (int i = 0; i < count; i++)
        {
            if (arg2[i].TryGetComponent(out IHealth health))
            {
                _damageInfo.hitDirection = transform.forward;
                _damageInfo.damagePosition = transform.position;
                health.DealDamage(_damageInfo);
            }
        }
    }

    public override void Attack()
    {
        overlapChecker.DetectOverlap();
    }
}
