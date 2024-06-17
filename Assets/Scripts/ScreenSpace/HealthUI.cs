using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour, IScreenSpaceUI
{
    [Header("UI")]
    [SerializeField] private Transform healthBar;
    [SerializeField] private Image outerFill;
    [SerializeField] private Image innerFill;

    [Header("Effect")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private int shakeAmount = 4;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float maxVisibleDistance = 10f;

    [SerializeField] private Transform target;
    private JuicerRuntimeCore<float> innerJuicer;
    private JuicerRuntime barScaleJuicer;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private float distance;

    public Transform Target => target;
    public float YOffset => yOffset;

    public bool IsActive => innerFill.fillAmount > 0 && innerFill.fillAmount < 1 && distance < maxVisibleDistance;

    public Action<float> GetDistanceFromCamera => HandleDistance;

    public Transform transfrom => transform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        innerJuicer = innerFill.JuicyFillAmount(0, duration);
        barScaleJuicer = healthBar.transform.JuicyScale(new Vector3(1.1f, 1.1f, 1.1f), shakeDuration).SetLoop(shakeAmount).SetEase(Ease.EaseInOutExpo);
    }

    public void Spawn(Transform target)
    {
        this.target = target;
    }

    public void ChangeValue(IHealth health)
    {
        outerFill.fillAmount = health.GetNormalisedHealth;
        barScaleJuicer.Start();
        innerJuicer.StartWithNewDestination(health.GetNormalisedHealth);
    }

    public void HandleDistance(float distance)
    {
        //this.distance = distance;
        //float alpha = Mathf.Lerp(1, 0.5f, distance / maxVisibleDistance);
        //canvasGroup.alpha = alpha;
    }

    public void ReturnToPool()
    {
 
    }
}
