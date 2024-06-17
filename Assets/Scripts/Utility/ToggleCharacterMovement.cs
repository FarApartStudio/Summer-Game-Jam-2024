using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCharacterMovement : MonoBehaviour
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
    }

    private void EnableMovement()
    {
        StoryModeManager.Instance.GetPlayer.gameObject.SetActive(true);
    }
}
