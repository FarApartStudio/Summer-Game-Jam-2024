using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiHitObserver : MonoBehaviour
{
    [SerializeField] private int minHits = 0;
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private HitObserver[] hitObservers;

    private int hitCount = 0;

    public UnityEvent GetOnComplete => OnComplete;

    private void Awake()
    {
        foreach (HitObserver hitObserver in hitObservers)
        {
            hitObserver.GetOnHit.AddListener(Hit);
        }

        if(minHits == 0)
        {
            minHits = hitObservers.Length;
        }
    }

    private void Hit()
    {
        hitCount++;

        if (hitCount >= minHits)
        {
            OnComplete.Invoke();
        }
    }

    public void ResetHit()
    {
        hitCount = 0;
    }

    [Button]
    private void GenerateHitObserver()
    {
        hitObservers = GetComponentsInChildren<HitObserver>();
    }
}
