using Pelumi.Juicer;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : GenericMenu<GameMenu>
{
    [Header("Crosshair")]
    [SerializeField] private Transform _crosshair;

    [Header("Aiming")]
    [SerializeField] private float maxAimSize = 3f;
    [SerializeField] private Transform _aimIndicator;
    [SerializeField] private Transform _hitMarker;

    private JuicerRuntimeCore<float> _fadeInOutEffect;
    private JuicerRuntimeCore<float> _fadeOutEffect;
    private JuicerRuntimeCore<float> _hitMarkerEffect;

    protected override void OnCreated()
    {
        _hitMarkerEffect = _hitMarker.JuicyScale(1f, .25f);
        _hitMarkerEffect.SetLoop(2);

        ToggleCrosshair (false);
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

    public void ToggleCrosshair (bool show)
    {
        _crosshair.parent.gameObject.SetActive(show);
    }

    public void SetAimIndicatorSize(float normalizedSize)
    {
        _aimIndicator.localScale = Vector3.one * normalizedSize * maxAimSize;
    }

    public void PlayHitMarker()
    {
        _hitMarkerEffect.StartFromOrigin();
    }
}