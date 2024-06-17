using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogManager;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private Dialog currentDialog;

    public void Play()
    {
        DialogManager.Instance.PlayDialog(currentDialog);
    }
}
