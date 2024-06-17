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
        int hitCount = 0;

        foreach (HitObserver hitObserver in hitObservers)
        {
            if (hitObserver.IsHit)
            {
                hitCount++;
            }
        }

        if (hitCount >= minHits)
        {

            Debug.Log("All hit observers are hit");
            OnComplete.Invoke();
        }
    }

    [Button]
    private void GenerateHitObserver()
    {
        hitObservers = GetComponentsInChildren<HitObserver>();
    }
}
