using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Pelumi.Juicer;

public class VideoController : MonoBehaviour
{
    public Action OnStarted;
    public Action OnCompleted;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine _playVideoCoroutine;

    public void PlayVideo()
    {
        _playVideoCoroutine = StartCoroutine(PlayVideo(videoClip));
    }

    public IEnumerator PlayVideo(VideoClip videoClip)
    {
        OnStarted?.Invoke();
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
        yield return new WaitForSecondsRealtime((float)videoClip.length + 1);
        OnCompleted?.Invoke();
    }

    public IEnumerator PlayVideo(VideoClip videoClip, IEnumerator OnComplected = null)
    {
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
        yield return new WaitForSecondsRealtime((float)videoClip.length + 1);
        if(OnComplected != null) yield return OnComplected;
    }

    public void SkipVideo()
    {
        if (_playVideoCoroutine != null)
        {
            StopCoroutine(_playVideoCoroutine);
            videoPlayer.Stop();
            OnCompleted?.Invoke();
            _playVideoCoroutine = null;
        }
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    public void SetVideoTime(float time)
    {
        videoPlayer.time = time;
    }

    public void SetVideoSpeed(float speed)
    {
        videoPlayer.playbackSpeed = speed;
    }

    public void SetVideoVolume(float volume)
    {
        videoPlayer.SetDirectAudioVolume(0, volume);
    }

    public void SetVideoLoop(bool loop)
    {
        videoPlayer.isLooping = loop;
    }

    public void SetVideoMute(bool mute)
    {
        videoPlayer.SetDirectAudioMute(0, mute);
    }

    public void SetVideoFullScreen(bool fullScreen)
    {
        videoPlayer.renderMode = fullScreen ? VideoRenderMode.CameraNearPlane : VideoRenderMode.MaterialOverride;
    }
}