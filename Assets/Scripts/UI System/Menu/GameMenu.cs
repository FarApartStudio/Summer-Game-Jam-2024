using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : GenericMenu<GameMenu>
{
    public Action OnSettingsPressed;
    public Action OnResumePressed;
    public Action OnQuitPressed;

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

    [Header("TaskPrompt")]
    [SerializeField] private Transform _taskPrompt;
    [SerializeField] private TextMeshProUGUI _taskPromptText;

    [Header ("TutorialPrompt")]
    [SerializeField] private Transform _tutorialPrompt;
    [SerializeField] private TextMeshProUGUI _tutorialActionText;
    [SerializeField] private Image _tutorialKeyImage;
    [SerializeField] private TextMeshProUGUI _tutorialActionDescription;

    [Header("Pause")]
    [SerializeField] private Transform _pauseMenu;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _quitBtn;


    private JuicerRuntimeCore<float> _fadeInOutEffect;
    private JuicerRuntimeCore<float> _fadeOutEffect;
    private JuicerRuntimeCore<float> _hitMarkerEffect;

    private JuicerRuntimeCore<float> _taskPromptEffect;
    private JuicerRuntimeCore<float> _tutorialPromptEffect;

    private JuicerRuntimeCore<float> _lowHealthEffect;

    protected override void OnCreated()
    {
        _hitMarker.localScale = Vector3.zero;
        _hitMarkerEffect = _hitMarker.JuicyScale(1f, .25f);
        _hitMarkerEffect.SetLoop(2);

        ToggleCrosshair (false);

        _taskPrompt.localScale = new Vector3(0, 1, 1);
        _taskPromptEffect = _taskPrompt.JuicyScaleX(1f, .25f);

        _tutorialPrompt.localScale = new Vector3(0, 1, 1);
        _tutorialPromptEffect = _tutorialPrompt.JuicyScaleX(1f, .25f);

        _lowHealthEffect = healthBar.transform.parent.JuicyScale(1.1f, .15f);
        _lowHealthEffect.SetLoop(0);

        _settingBtn.onClick.AddListener(() => OnSettingsPressed?.Invoke());
        _resumeBtn.onClick.AddListener(() => OnResumePressed?.Invoke());
        _quitBtn.onClick.AddListener(() => OnQuitPressed?.Invoke());
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

        if (value < 0.3f)
        {
            if (!_lowHealthEffect.IsPlaying)
            {
                _lowHealthEffect.Start();
            }

            if (value == 0)
            {
                if (_lowHealthEffect.IsPlaying)
                {
                    _lowHealthEffect.Stop();
                }
            }
        }
        else
        {
            if (_lowHealthEffect.IsPlaying)
            {
                _lowHealthEffect.Stop();
            }
        }
    }

    public void SetMissionPrompt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _taskPromptEffect.StartWithNewDestination(0);
            return;
        }
        _taskPromptEffect.StartWithNewDestination(1);
        _taskPromptText.text = text;
    }

    public void SetTutorialPrompt(TutorialKeyPromptData data)
    {
        if (string.IsNullOrEmpty(data.ActionText))
        {
            _tutorialPromptEffect.StartWithNewDestination(0);
            return;
        }

        _tutorialPromptEffect.StartWithNewDestination(1);
        _tutorialActionText.text = data.ActionText;
        _tutorialKeyImage.sprite = data.keyImage;
        _tutorialActionDescription.text = data.ActionDescription;
    }

    public void TogglePause (bool state)
    {
        _pauseMenu.gameObject.SetActive(state);
        Time.timeScale = state ? 0 : 1;
    }
}