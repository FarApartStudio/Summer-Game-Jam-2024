using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuicyFillBar : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float shakeDuration = 0.5f;

    [SerializeField] Transform healthBar;
    [SerializeField] private Image outerFill;
    [SerializeField] private Image innerFill;

    private JuicerRuntimeCore<float> innerJuicer;
    private JuicerRuntimeCore<float> outterJuicer;
    private JuicerRuntime barRotateJuicer;
    private JuicerRuntime barScaleJuicer;

    [SerializeField]  float normalisedValue = 0;

    private void Start()
    {
        innerJuicer = innerFill.JuicyFillAmount(0, duration);
        outterJuicer = outerFill.JuicyFillAmount(0, duration);

        barRotateJuicer = healthBar.JuicyLocalRotate(new Vector3(0, 0, -10), shakeDuration).SetLoop(2).SetEase(Ease.EaseInCubic);
        barScaleJuicer = healthBar.JuicyScale(new Vector3(1.1f, 0.8f, 1.1f), shakeDuration).SetLoop(2).SetEase(Ease.EaseInOutExpo);

        normalisedValue = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeValueInc(normalisedValue += 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeValueDec(normalisedValue -= 0.2f);
        }
    }

    public void ChangeValueDec(float normalisedValue)
    {
        innerJuicer.Stop();
        outterJuicer.Stop();

        outerFill.fillAmount = normalisedValue;
        barRotateJuicer.Start();
        barScaleJuicer.Start();
        innerJuicer.StartWithNewDestination(normalisedValue);
    }

    public void ChangeValueInc(float normalisedValue)
    {
        innerJuicer.Stop();
        outterJuicer.Stop();

        innerFill.fillAmount = normalisedValue;
        barRotateJuicer.Start();
        barScaleJuicer.Start();
        outterJuicer.StartWithNewDestination(normalisedValue);
    }
}
