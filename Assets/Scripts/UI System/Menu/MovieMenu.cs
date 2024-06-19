using Pelumi.Juicer;
using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MovieMenu : GenericMenu<MovieMenu>
{
    [SerializeField] private CanvasGroup _content;

    private JuicerRuntimeCore<float> _fadeInEffect;
    private VideoController _videoController;

    public VideoController GetVideoController => _videoController;

    protected override void OnCreated()
    {
        _videoController = GetComponent<VideoController>();
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

    public void Play(VideoClip videoClip, Action onStart, Action onFinished)
    {
        Open();
        _videoController.PlayVideo(videoClip, onStart, onFinished);
    }

    private void Update()
    {
        if (InputManager.Instance.GetJumpInput())
        {
            _videoController.SkipVideo();
        }
    }

}