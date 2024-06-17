using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;
using UnityEngine.Events;

[System.Serializable]
public class DelayedUnityEvent
{
    [SerializeField] private float _delay;
    [SerializeField] private UnityEvent _event;

    public DelayedUnityEvent(UnityEvent unityEvent, float delay)
    {
        _event = unityEvent;
        _delay = delay;
    }

    public void Invoke()
    {
        Juicer.StartCoroutine(InvokeAfterDelay());
    }

    private IEnumerator InvokeAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        _event?.Invoke();
    }
}