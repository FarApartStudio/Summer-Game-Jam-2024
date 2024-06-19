using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCharacterVisibility : MonoBehaviour
{

    public void TogglePlayerVisibility (bool value)
    {
        StoryModeManager.Instance.GetPlayer.gameObject.SetActive(value);
    }

    public void ToggleMovement(bool value)
    {
        if (value)
        {
            EnableMovement();
        }
        else
        {
            DisableMovement();
        }
    }

    public void ToggleMovementOnly(bool value)
    {
        StoryModeManager.Instance.GetPlayer.ToggleMovement(value);
    }

    private void DisableMovement()
    {
        StoryModeManager.Instance.GetPlayer.ToggleMovement(false);
        UIManager.Instance.ToggleVisibility(Visibility.Invisible);
    }

    private void EnableMovement()
    {
        StoryModeManager.Instance.GetPlayer.ToggleMovement(true);
        UIManager.Instance.ToggleVisibility(Visibility.Visible);
    }
}
