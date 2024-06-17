using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TutorialPromptData
{
    public string ActionText;
    public string ActionKey;
    public string ActionDescription;

    public TutorialPromptData(string actionText, string actionKey, string actionDescription)
    {
        ActionText = actionText;
        ActionKey = actionKey;
        ActionDescription = actionDescription;
    }
}

public class TurotialPrompt : MonoBehaviour
{
    public static Action<TutorialPromptData> OnTutorialPrompt;

    [SerializeField] private TutorialPromptData _tutorialPromptData;

    public void ShowTutorialPrompt()
    {
        OnTutorialPrompt?.Invoke(_tutorialPromptData);
    }   
}
