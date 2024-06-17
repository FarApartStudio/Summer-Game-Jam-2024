using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCharacterVisibility : MonoBehaviour
{

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

    private void DisableMovement()
    {
        StoryModeManager.Instance.GetPlayer.gameObject.SetActive(false);
        UIManager.Instance.ToggleVisibility(Visibility.Invisible);
    }

    private void EnableMovement()
    {
        StoryModeManager.Instance.GetPlayer.gameObject.SetActive(true);
        UIManager.Instance.ToggleVisibility(Visibility.Visible);
    }
}
