using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TotemPuzzle : MonoBehaviour
{
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private UnityEvent OnComplete;
    [SerializeField] private Totem[] totems;

    private void Awake()
    {
        if (totems == null || totems.Length == 0)
            GenerateTotems();

        foreach (var totem in totems)
        {
            totem.SetUp(resetDelay);
            totem.GetHitEvent().AddListener(OnTotemHit);
        }
    }

    [Button]
    private void GenerateTotems ()
    {
        totems = GetComponentsInChildren<Totem>();
    }

    private void OnTotemHit()
    {
        if (!IsPuzzleCompleted()) return;
        MarkAllAsComplete();
        OnComplete.Invoke();
    }

    public void MarkAllAsComplete ()
    {
        foreach (var totem in totems)
        {
            totem.OnSucessFul();
        }
    }

    public bool IsPuzzleCompleted()
    {
        foreach (var totem in totems)
        {
            if (!totem.IsHit())
                return false;
        }
        return true;
    }
}
