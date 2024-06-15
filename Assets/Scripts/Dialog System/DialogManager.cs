using System.Collections;
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
        if (!isFinished) return;
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

    IEnumerator PlayVoiceOver(AudioClip audioClip)
    {
        isPlayingVoiceOver = true;
        audioSource.PlayOneShot(audioClip);
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

            yield return new WaitForSecondsRealtime(textSequences[i].audioClip.length);

            StartCoroutine(PlayText());

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
        StopAllCoroutines();
        popUpBox.SetActive(false);
        currentDialog.OnFinsihed.Invoke();
    }
}