using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private VideoController _videoController;


    private void Awake()
    {
        _videoController.OnStarted = OnVideoStarted;
        _videoController.OnCompleted = OnVideoCompleted;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _videoController.PlayVideo();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _videoController.SkipVideo();
        }
    }

    private void OnVideoStarted()
    {

    }

    private void OnVideoCompleted()
    {
        LoadingScreen.Instance.LoadScene(1);
    }
}
