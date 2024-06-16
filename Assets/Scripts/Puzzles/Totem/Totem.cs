using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Totem : MonoBehaviour, IDamageable
{
    [SerializeField] private UnityEvent OnHitWhenNotReset;
    [SerializeField] private UnityEvent OnReset;
    [SerializeField] private UnityEvent OnComplete;

    private float resetDelay;
    private bool isHit;
    private bool isCompleted;
    private float timer;

    public UnityEvent GetHitEvent() => OnHitWhenNotReset;
    public bool IsHit() => isHit;

    private void Update()
    {
        HandleTimer();
    }

    public void Damage(DamageInfo damageInfo, Vector3 damagePosition)
    {
        if (isCompleted || isHit)
            return;
        isHit = true;
        OnHitWhenNotReset.Invoke();
    }

    public void SetUp(float resetDelay)
    {
        this.resetDelay = resetDelay;
    }

    public void HandleTimer ()
    {
        if (!isCompleted && isHit)
        {
            timer += Time.deltaTime;
            if (timer >= resetDelay)
            {
                Refresh();
            }
        }
    }

    private void Refresh()
    {
        isHit = false;
        OnReset.Invoke();
        timer = 0;
    }

    public void OnSucessFul ()
    {
        isCompleted = true;
        isHit = false;
        OnComplete.Invoke();
    }
}
