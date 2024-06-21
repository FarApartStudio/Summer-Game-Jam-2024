using Pelumi.AudioSystem;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : GenericMenu<MainMenu>
{
    public Action OnNewGameBtnPressed;
    public Action OnContinueBtnPressed;
    public Action OnQuitBtnPressed;
    public Action OnExitBtnPressed;


    [SerializeField] private Transform _content;
    [SerializeField] private Transform _mainBtns;
    [SerializeField] private AdvanceButton _quitBtn;
    [SerializeField] private AdvanceButton _newGameBtn;
    [SerializeField] private AdvanceButton _continueBtn;
    [SerializeField] private AdvanceButton _creditsBtn;
    [SerializeField] private AdvanceButton _hidecreditsBtn;

    [Header("Audio")]
    [SerializeField] private AudioSystemSO _audioSettings;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Credits")]
    [SerializeField] private CanvasGroup _creditsPanel;
    [SerializeField] private Transform _creditsContent;

    private JuicerRuntimeCore<float> creditPanelOpenEffect;
    private JuicerRuntimeCore<float> _creditContentOpenEffect;

    private JuicerRuntimeCore<float> _mainButtonEffect;

    protected override void OnCreated()
    {
        _quitBtn.onClick.AddListener(() => OnQuitBtnPressed?.Invoke());
        _newGameBtn.onClick.AddListener(() => OnNewGameBtnPressed?.Invoke());
        _continueBtn.onClick.AddListener(() => OnContinueBtnPressed?.Invoke());
        _creditsBtn.onClick.AddListener(() => ShowCredits(true));
        _hidecreditsBtn.onClick.AddListener(() => ShowCredits(false));


        _creditsPanel.alpha = 0;
        _creditsPanel.interactable = false;
        creditPanelOpenEffect = _creditsPanel.JuicyAlpha(1, 0.25f);

        _creditsContent.transform.localScale = Vector3.zero;
        _creditContentOpenEffect = _creditsContent.JuicyScale(1, 0.25f);
        _creditContentOpenEffect.SetEase(Ease.Spring).SetDelay(0.1f);

        _mainButtonEffect = _mainBtns.JuicyScale(1, 0.25f);
        _mainButtonEffect.SetEase(Ease.Spring);

        _masterVolumeSlider.onValueChanged.AddListener((value) => _audioSettings.ChangeVolume(AudioCategory.Master, value));
        _musicVolumeSlider.onValueChanged.AddListener((value) => _audioSettings.ChangeVolume(AudioCategory.Music, value));
        _sfxVolumeSlider.onValueChanged.AddListener((value) => _audioSettings.ChangeVolume(AudioCategory.Sfx, value));
    }

    protected override void OnOpened()
    {
        _masterVolumeSlider.value = _audioSettings.GetVolume(AudioCategory.Master);
        _musicVolumeSlider.value = _audioSettings.GetVolume(AudioCategory.Music);
        _sfxVolumeSlider.value = _audioSettings.GetVolume(AudioCategory.Sfx);

        _continueBtn.gameObject.SetActive(PlayerPrefs.GetInt("CurrentArea", 1) > 1);
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

    private void ShowCredits(bool show)
    {
        _creditContentOpenEffect.StartWithNewDestination(show ? 1 : 0);
        _mainButtonEffect.StartWithNewDestination(show ? 0 : 1);
        creditPanelOpenEffect.StartWithNewDestination(show ? 1 : 0);
        _creditsPanel.interactable = show;
        _creditsPanel.blocksRaycasts = show;
    }
}