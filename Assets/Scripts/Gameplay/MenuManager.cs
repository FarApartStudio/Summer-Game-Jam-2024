using Pelumi.AudioSystem;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private VideoClip introVideo;

    private MainMenu _mainMenu;

    private void Start()
    {
        _mainMenu = UIManager.GetMenu<MainMenu>();

        _mainMenu.OnNewGameBtnPressed = () =>
        {
            PlayerPrefs.SetInt("CurrentArea", 1);

            _mainMenu.Close();
            UIManager.GetMenu<MovieMenu>().Play(introVideo, OnVideoStarted, OnVideoCompleted);
        };

        _mainMenu.OnContinueBtnPressed = () =>
        {
            _mainMenu.Close();
            UIManager.GetMenu<MovieMenu>().Play(introVideo, OnVideoStarted, OnVideoCompleted);
        };

        _mainMenu.OnQuitBtnPressed = () =>
        {
            Application.Quit();
        };

        InputManager.Instance.ToggleCursor(true);

        _mainMenu.Open();

        AudioSystem.PlayAudio(AudioTypeID.MenuMusic, AudioCategory.Music);
    }

    private void OnVideoStarted()
    {
        AudioSystem.StopAudioWithFade(AudioCategory.Music);
    }

    private void OnVideoCompleted()
    {
        LoadingScreen.Instance.LoadScene(1);
    }
}
