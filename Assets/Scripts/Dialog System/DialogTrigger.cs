using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogManager;

[RequireComponent(typeof(InteractionHandler))]
public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private Dialog currentDialog;

    InteractionHandler interactionHandler;

    private void Awake()
    {
        interactionHandler = GetComponent<InteractionHandler>();
        interactionHandler.OnInteractStart += PlayDialog;
    }

    private void PlayDialog(Collider collider)
    {
        DialogManager.Instance.PlayDialog(currentDialog);
    }
}
