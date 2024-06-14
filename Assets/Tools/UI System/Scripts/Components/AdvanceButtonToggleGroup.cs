using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceButtonToggleGroup : MonoBehaviour
{
    [SerializeField] bool allowUnToggle = false;
    private List<AdvanceButton> advanceButtons = new List<AdvanceButton>();
    private AdvanceButton selectedButton;
    public bool AllowUnToggle => allowUnToggle;
    public AdvanceButton SelectedButton => selectedButton;

    public void AddButton (AdvanceButton advanceButton)
    {
        advanceButtons.Add(advanceButton);
        advanceButton.OnToggle.AddListener((status) => OnButtonToggle(advanceButton, status));
    }

    private void OnButtonToggle(AdvanceButton advanceButton , bool status)
    {
        if (!status)
        {
            return;
        }

        selectedButton = advanceButton;

        foreach (AdvanceButton button in advanceButtons)
        {
            if (button != advanceButton)
            {
                button.Toggle(false);
            }
        }
    }
}
