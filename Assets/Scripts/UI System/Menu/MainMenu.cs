using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : GenericMenu<GameMenu>
{
    public Action OnSettingsPressed;
    public Action OnResumePressed;
    public Action OnQuitPressed;


    [SerializeField] private Transform _pauseMenu;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _quitBtn;


    private JuicerRuntimeCore<float> _fadeInOutEffect;

    protected override void OnCreated()
    {


        //  _settingBtn.onClick.AddListener(() => OnSettingsPressed?.Invoke());
        //  _resumeBtn.onClick.AddListener(() => OnResumePressed?.Invoke());
        //  _quitBtn.onClick.AddListener(() => OnQuitPressed?.Invoke());
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
}