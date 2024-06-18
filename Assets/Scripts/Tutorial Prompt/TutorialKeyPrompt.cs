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

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
