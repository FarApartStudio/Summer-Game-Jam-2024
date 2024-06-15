using Pelumi.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationMenu : GenericMenu<ConfirmationMenu>
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private AdvanceButton yesButton;
    [SerializeField] private AdvanceButton noButton;

    Action yesAction;
    Action noAction;

    protected override void OnCreated()
    {
        yesButton.onClick.AddListener( () =>
        {
            yesAction?.Invoke();
            Close();
        });

        noButton.onClick.AddListener(() =>
        {
            noAction?.Invoke();
            Close();
        });
    }

    protected override void OnOpened()
    {

    }

    protected override void OnClosed()
    {

    }

    protected override void OnDestoryInvoked()
    {

    }

    public void Display(string title, string message, Action yesAction, Action noAction = null)
    {
        this.title.text = title;
        this.message.text = message;
        this.yesAction = yesAction;
        this.noAction = noAction;
        Open();
    }

    public override void ResetMenu()
    {

    }
}
