using System;
using System.Collections;
using UnityEngine;

public class CountDownTimer
{
    event Action<int> OnTimeTickInt;
    event Action<float> onTimeTick = null;
    event Action onTimeLapse = null;

    private MonoBehaviour owner;
    private bool paused = false;
    private float currentTime = 0;
    private Coroutine timerRoutine = null;

    private float maxTime = 0;
    private float lastTime = 0;

    public CountDownTimer(MonoBehaviour monoBehaviour)
    {
        owner = monoBehaviour;
    }
      
    public void Start(float _time, Action<float> _onTimeTick = null, Action _onTimeLapse = null, Action<int> _onTimeTickInt = null)
    {
        if (timerRoutine != null)
            return;

        maxTime = _time;
        currentTime = maxTime;
        onTimeTick =null;
        onTimeLapse =null;
        OnTimeTickInt = null;
        onTimeTick += _onTimeTick;
        onTimeLapse += _onTimeLapse;
        OnTimeTickInt += _onTimeTickInt;
        timerRoutine = owner.StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (true)
        {
            if(!paused)
            {
                lastTime = currentTime;
                currentTime -= Time.deltaTime;
                onTimeTick?.Invoke(currentTime);

                if ((int)currentTime != (int)GetLastTime)
                {
                    OnTimeTickInt?.Invoke((int)currentTime);
                }

                if (currentTime <= 0)
                {
                    Stop();
                    onTimeLapse?.Invoke();
                }
            }

            yield return null;
        }
    }

    public void Pause() => paused = true;

    public void Resume() => paused = false;

    public float GetCurrentTime => currentTime;

    public float GetMaxTime => maxTime;

    public float GetNormalizedTime => currentTime / maxTime;

    public float GetLastTime => lastTime;

    public bool Stop()
    {
        if (timerRoutine == null)
            return false;
        
        owner.StopCoroutine(timerRoutine);
        onTimeTick?.Invoke(0);
        OnTimeTickInt?.Invoke(0);
        timerRoutine = null;
        return true;
    }

    public void Complete()
    {
        if (Stop())
        onTimeLapse?.Invoke();
    }

    public void Restart()
    {
        Stop();
        Start(maxTime, onTimeTick, onTimeLapse, OnTimeTickInt);
    }
}
