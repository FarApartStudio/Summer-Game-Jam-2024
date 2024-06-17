using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : GenericMenu<GameMenu>
{
    [Header("Crosshair")]
    [SerializeField] private Transform _crosshair;

    [Header("Aiming")]
    [SerializeField] private float maxAimSize = 3f;
    [SerializeField] private Transform _aimIndicator;
    [SerializeField] private Transform _hitMarker;

    [Header("DamageIndicator")]
    [SerializeField] private Transform _damageSpawnPos;
    [SerializeField] private DamageIndicatorUI _damageIndicatorPrefab;

    [Header("PlayerHealth")]
    [SerializeField] private Image healthBar;

    private JuicerRuntimeCore<float> _fadeInOutEffect;
    private JuicerRuntimeCore<float> _fadeOutEffect;
    private JuicerRuntimeCore<float> _hitMarkerEffect;

    protected override void OnCreated()
    {
        _hitMarker.localScale = Vector3.zero;
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

    public void SpawnDamageIndicator(Transform player, Vector3 damagePos)
    {
        DamageIndicatorUI damageIndicator = ObjectPoolManager.SpawnObject(_damageIndicatorPrefab, _damageSpawnPos);
        (damageIndicator.transform as RectTransform).anchoredPosition = Vector3.zero;
        damageIndicator.SetUp(player, damagePos);
    }

    public void SetPlayerHealthBar(float value)
    {
        healthBar.fillAmount = value;
    }
}