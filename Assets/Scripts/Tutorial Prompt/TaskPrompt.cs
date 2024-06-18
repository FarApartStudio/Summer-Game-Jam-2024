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

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
