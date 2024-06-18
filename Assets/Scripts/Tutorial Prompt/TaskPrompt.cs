using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPrompt : MonoBehaviour
{
    public static Action<string> OnMissionPrompt;

    [SerializeField] private string missionPromptText;

    public string MissionPromptText => missionPromptText;

    public void ShowMissionPrompt()
    {
        OnMissionPrompt?.Invoke(missionPromptText);
    }
}
