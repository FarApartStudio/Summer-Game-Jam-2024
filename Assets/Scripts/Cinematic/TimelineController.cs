using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TimelineEvent
{
    public bool IsTriggered;
    public float NormalisedTime;
    public Action Action;
}

public class TimelineController : MonoBehaviour
{
    public event Action<PlayableDirector> OnStart;
    public event Action<PlayableDirector> OnPaused;
    public event Action OnFinished;

    [SerializeField] private bool playOnStart;
    [SerializeField] private List<TimelineEvent> timelineEvents;
    private PlayableDirector playableDirector;

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
        StartCoroutine(PlayTimeRoutine((float)playableDirector.duration));
    }

    public void AddTimeEvent(float normalisedTime, Action Action)
    {
        timelineEvents.Add(new TimelineEvent { Action = Action, NormalisedTime = normalisedTime });
    }

    IEnumerator PlayTimeRoutine (float time)
    {
        while (playableDirector.time < time)
        {
            foreach (var timelineEvent in timelineEvents)
            {
                if (!timelineEvent.IsTriggered && playableDirector.time >= playableDirector.duration * timelineEvent.NormalisedTime)
                {
                    if (!timelineEvent.IsTriggered)
                    {
                        timelineEvent.IsTriggered = true;
                        timelineEvent.Action?.Invoke();
                    }
                }
            }
            yield return null;
        }

        OnFinished?.Invoke();
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
      //  OnFinished?.Invoke(director);
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
