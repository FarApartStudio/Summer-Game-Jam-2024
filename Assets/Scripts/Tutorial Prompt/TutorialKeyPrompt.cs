using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TutorialKeyPromptData
{
    public string ActionText;
    public Sprite keyImage;
    public string ActionDescription;
}

public class TutorialKeyPrompt : MonoBehaviour
{
    public static Action<TutorialKeyPromptData> OnTutorialPrompt;

    [SerializeField] private TutorialKeyPromptData _tutorialPromptData;

    public void ShowTutorialPrompt()
    {
        OnTutorialPrompt?.Invoke(_tutorialPromptData);
    }   
}
