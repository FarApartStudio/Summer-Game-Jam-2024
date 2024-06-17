using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FocusCamera : MonoBehaviour
{
    [SerializeField] private float _timeToDisable = 1f;
    [SerializeField] private DelayedUnityEvent OnEnable;
    [SerializeField] private DelayedUnityEvent OnDisable;

    public void Show()
    {
        OnEnable?.Invoke();
        gameObject.SetActive(true);
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(_timeToDisable);
        gameObject.SetActive(false);
        OnDisable?.Invoke();
    }
}
