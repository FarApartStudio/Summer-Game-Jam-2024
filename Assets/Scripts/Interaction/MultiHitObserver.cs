using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiHitObserver : MonoBehaviour
{
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private HitObserver[] hitObservers;

    private void Awake()
    {
        foreach (HitObserver hitObserver in hitObservers)
        {
            hitObserver.GetOnHit.AddListener(Hit);
        }
    }

    private void Hit()
    {
        foreach (HitObserver hitObserver in hitObservers)
        {
            if (!hitObserver.IsHit) return;
        }

        OnComplete.Invoke();
    }
}
