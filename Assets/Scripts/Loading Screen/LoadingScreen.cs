using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Random = UnityEngine.Random;
using System.Text;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public event Action OnLoadingStarted;

    [SerializeField] float minTimeToLoad = 2f;
    [SerializeField] public GameObject loaderCanvas;
    [SerializeField] Slider loadingBar;


    [Header("Debug")]
    [SerializeField] private float loadingProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void LoadScene(int sceneIndex)
    {
        OnLoadingStarted?.Invoke();
        Time.timeScale = 1;
        loadingProgress = 0;
        StartCoroutine(LoadAsychronously(sceneIndex));
    }

    IEnumerator LoadAsychronously(int sceneIndex)
    {
        loadingBar.value = 0;

        AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(sceneIndex);
        sceneToLoad.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        float normalizedTime = 0;

        while (!sceneToLoad.isDone && normalizedTime < 1) 
        {
            loadingProgress += Time.unscaledDeltaTime / minTimeToLoad;
            normalizedTime = loadingProgress / minTimeToLoad;
            float adjustedNormalizedTime = Mathf.Min(sceneToLoad.progress, normalizedTime);

            loadingBar.value = adjustedNormalizedTime + .1f;

            yield return null;
        }

        sceneToLoad.allowSceneActivation = true;

        yield return new WaitForEndOfFrame();

        loaderCanvas.SetActive(false);
    }

    public void RegisterOnStartedEvent(Action action)
    {
        OnLoadingStarted += action;
    }

    public void UnRegisterOnStartedEvent(Action action)
    {
        OnLoadingStarted -= action;
    }
}
