using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class GameEffectController : MonoBehaviour
{
    public static GameEffectController Instance;

    Coroutine stopTimeCoroutine;
    Coroutine stopFrameCoroutine;
    private bool stoppingTime = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ForceStop(Action OnFinished)
    {
        stoppingTime = true;
    
        if (stopTimeCoroutine != null)
        {
            StopCoroutine(stopTimeCoroutine);
            stopTimeCoroutine = null;
        }

      
        Time.timeScale = 0.2f;

        Juicer.WaitForSecondsRealtime (2f, new JuicerCallBack(() =>
        {
            Time.timeScale = 1f;
            stoppingTime = false;
            OnFinished?.Invoke();
        }));
    }

    public void StopTime(float newTimeScale,float restoreSpeed, float duration)
    {
        if (stoppingTime)
            return;
        
        Time.timeScale = newTimeScale;
        if (stopTimeCoroutine != null) return;
        stopTimeCoroutine = StartCoroutine(StopTimeCoroutine(restoreSpeed, duration));
    }

    private IEnumerator StopTimeCoroutine(float restoreSpeed, float duration)
    {  
        yield return new WaitForSecondsRealtime(duration);
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.unscaledDeltaTime * restoreSpeed;
            yield return null;
        }
     
        Time.timeScale = 1;
        stopTimeCoroutine = null;
        stoppingTime = false;
    }

    public void SlowMo(float slowMoFactor, float duration)
    {
        StartCoroutine(SlowMoCoroutine(slowMoFactor, duration));
    }

    private IEnumerator SlowMoCoroutine(float slowMoFactor, float duration)
    {
        // Immediately reduce the time scale to the slowMoFactor
        Time.timeScale = slowMoFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float elapsedTime = 0f;

        // Calculate the time to spend in slow motion
        float slowMoDuration = duration * (slowMoFactor / (1 - slowMoFactor));

        // Gradually restore the time scale back to normal over the remaining duration
        float restoreDuration = duration - slowMoDuration;

        // Wait for the slow motion duration
        while (elapsedTime < slowMoDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Gradually restore the time scale back to normal
        while (elapsedTime < restoreDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowMoFactor, 1f, elapsedTime / restoreDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        // Ensure the time scale is fully restored to normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public void StopFrame(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        if (stopFrameCoroutine != null) return;
        stopFrameCoroutine = StartCoroutine(StopTimeCoroutine());
    }

    private IEnumerator StopTimeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Time.timeScale = 1;
        stopFrameCoroutine = null;
    }
}
