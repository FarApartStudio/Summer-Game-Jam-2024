using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pelumi.Juicer;

public class ScreenFadeMenu : GenericMenu<ScreenFadeMenu>
{
    private Action _OnFadeStart;
    private Action _OnFadeMid;
    private Action _OnFadeEnd;

    private JuicerRuntimeCore<float> _fadeInOutEffect;
    private JuicerRuntimeCore<float> _fadeOutEffect;

    protected override void OnCreated()
    {
        _fadeInOutEffect = _canvasGroup.JuicyAlpha(1, 1);

        _fadeInOutEffect.SetLoop(2);

        _fadeInOutEffect.SetOnStart(() =>
        {
            _canvasGroup.blocksRaycasts = true;
            _OnFadeStart?.Invoke();
        });

        _fadeInOutEffect.SetOnStepComplete(() =>
        {
            _OnFadeMid?.Invoke();
        });

        _fadeInOutEffect.SetOnCompleted(() =>
        {
            _OnFadeEnd?.Invoke();
            _canvasGroup.blocksRaycasts = false;
        });

        _fadeOutEffect = _canvasGroup.JuicyAlpha(0, 1);
        _fadeOutEffect.SetOnCompleted(() =>
        {
            _OnFadeEnd?.Invoke();
            _canvasGroup.blocksRaycasts = false;
        });
    }

    protected override void OnOpened()
    {

    }

    protected override void OnClosed()
    {

    }

    protected override void OnDestoryInvoked()
    {

    }

    public override void ResetMenu()
    {

    }

    public void Show(float duration = 1, float waitDelay = 1, Action OnFadeStart = null, Action OnFadeMid = null, Action OnFadeEnd = null)
    {
        _OnFadeStart = OnFadeStart;
        _OnFadeMid = OnFadeMid;
        _OnFadeEnd = OnFadeEnd;

        _canvasGroup.alpha = 0;

        _fadeInOutEffect.SetStepDelay(waitDelay);
        _fadeInOutEffect.SetDuration(duration);
        _fadeInOutEffect.Start();
    }
}
