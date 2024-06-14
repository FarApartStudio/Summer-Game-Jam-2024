using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public event Action<PlayableDirector> OnStart;
    public event Action<PlayableDirector> OnPaused;
    public event Action<PlayableDirector> OnFinished;

    public bool playOnStart;
    PlayableDirector playableDirector;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playableDirector.played += OnPlayableDirectorPlayed;
        playableDirector.paused += OnPlayableDirectorPaused;
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    private void Start()
    {
        if (playOnStart)
        Play();
    }

    public void Play()
    {
        playableDirector.Play();
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        OnFinished?.Invoke(director);
    }

    private void OnPlayableDirectorPaused(PlayableDirector director)
    {
        OnPaused?.Invoke(director);
    }

    private void OnPlayableDirectorPlayed(PlayableDirector director)
    {
        OnStart?.Invoke(director);
    }
}
