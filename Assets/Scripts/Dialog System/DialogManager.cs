﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialog
    {
        public DialogSO dialogObject;
        public UnityEvent OnBegin;
        public UnityEvent OnFinsihed;
    }

    public static DialogManager Instance { get; private set; }

    [Header("Properties")]
    [SerializeField] private GameObject popUpBox;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI dialogBoxText;
    [SerializeField] private float delay = 0.1f;

    [Header("Debug")]
    private Dialog currentDialog;
    private TextMeshProUGUI dialogText;
    private string fullText;
    private string currentText = "";
    private bool isTyping = false;
    private bool isPlayingVoiceOver = false;
    private bool isFinished = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialogBoxText.text = "";
        dialogText = dialogBoxText;
    }

    public void PlayDialog(Dialog dialog)
    {
        if (!isFinished)
        {
            SkipDialog();
        }
        currentDialog = dialog;
        currentDialog.OnBegin?.Invoke();
        popUpBox.SetActive(true);
        StartCoroutine(DialogRoutine(currentDialog.dialogObject.textSequences));
    }

    IEnumerator PlayText()
    {
        isTyping = true;
        foreach (char letter in fullText)
        {
            currentText = dialogText.text += letter;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }

    IEnumerator PlayOverDurationText(float duration)
    {
        isTyping = true;
        float time = 0;
        int charIndex = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            charIndex = (int)Mathf.Lerp(0, fullText.Length, time / duration);
            dialogText.text = fullText.Substring(0, charIndex);
            yield return null;
        }
        dialogText.text = fullText;
        isTyping = false;
    }

    IEnumerator PlayVoiceOver(AudioClip audioClip)
    {
        isPlayingVoiceOver = true;
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(audioClip.length);
        isPlayingVoiceOver = false;
    }

    IEnumerator DialogRoutine( DialogSO.TextSequence[] textSequences)
    {
        isFinished = false;
        for (int i = 0; i < textSequences.Length; i++)
        {
            dialogBoxText.text = "";
            fullText = textSequences[i].messages;

            isTyping = true;
            isPlayingVoiceOver = true;

           // StartCoroutine(PlayText());

            StartCoroutine(PlayOverDurationText(textSequences[i].audioClip.length - 0.5f));

            StartCoroutine(PlayVoiceOver(textSequences[i].audioClip));

            yield return new WaitUntil(() => !isTyping && !isPlayingVoiceOver);
        }


        yield return new WaitForSecondsRealtime(1);

        popUpBox.SetActive(false);
        currentDialog.OnFinsihed?.Invoke();
        isFinished = true;
    }

    
    public void SkipDialog()
    {
        audioSource.Stop();
        StopAllCoroutines();
        popUpBox.SetActive(false);
        currentDialog.OnFinsihed.Invoke();
    }
}