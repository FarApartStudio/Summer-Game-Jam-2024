using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedHazardDamager : MonoBehaviour
{
    [SerializeField] private float damageInterval;
    [SerializeField] private int damage;
    [SerializeField] private bool knockback;

    private OverlapChecker overlapChecker;
    private float timer;
    private bool isOn;

    private void Awake()
    {
        overlapChecker = GetComponent<OverlapChecker>();
    }

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;
    }

    private void Update()
    {
        if (isOn)
        {
            timer += Time.deltaTime;
            if (timer >= damageInterval)
            {
                DealDamage();
                timer = 0;
            }
        }
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

    private void DealDamage()
    {
        overlapChecker.DetectOverlap();
    }

    public void TurnOn()
    {
        isOn = true;
    }

    public void TurnOff()
    {
        isOn = false;
    }
}
