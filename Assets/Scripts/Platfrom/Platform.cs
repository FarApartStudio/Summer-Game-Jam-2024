using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pelumi.Juicer;

public class Platform : MonoBehaviour
{
    [SerializeField] private float activeTime = 3f;
    [SerializeField] private UnityEvent OnPlatfromTrigger;
    [SerializeField] private Collider _collider;
    [SerializeField] InteractionHandler interactionHandler;

    private bool isTriggered;
    private JuicerRuntimeCore<float> triggerEffect;
    private JuicerRuntimeCore<Vector3> scaleEffect;

    private Vector3 _localPos;

    private void Awake()
    {
        _localPos = transform.localPosition;
        transform.localScale = Vector3.zero;
        interactionHandler.OnInteractStart += OnInteractStart;

        triggerEffect = transform.JuicyLocalMoveY(_localPos.y - 0.2f, 0.2f);
        triggerEffect.SetEase(Ease.Spring);

        scaleEffect = transform.JuicyScale(Vector3.one, 0.25f);
        scaleEffect.SetEase(Ease.Spring);
    }

    private void OnInteractStart(Collider collider)
    {
        if (isTriggered) return;
        isTriggered = true;
        OnPlatfromTrigger?.Invoke();
        StartCoroutine(DeactivatePlatform());
        triggerEffect.Start();
    }

    private IEnumerator DeactivatePlatform()
    {
        yield return new WaitForSeconds(activeTime);
        _collider.enabled = false;
        scaleEffect.StartWithNewDestination(Vector3.zero);
    }

    public void ResetPlatfrom ()
    {
        StopAllCoroutines();
        isTriggered = false;
        transform.localPosition = _localPos;
        scaleEffect.StartWithNewDestination(Vector3.one);
        _collider.enabled = true;
    }
}
