using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotAnimatorController : CharacterAnimatorController
{
    public Action OnClimbElevate;
    public Action OnRefillAmmo;
    public Action OnReload;
    [SerializeField] private float gravityMultiplier = 1f;

    private void OnAnimatorMove()
    {
        if (characterManager.ApplyRootMotion)
        {
            Vector3 velocity = characterManager.GetAnimator.deltaPosition;
            characterManager.transform.rotation *= characterManager.GetAnimator.deltaRotation;

            if (characterManager.ApplyRootGravity)
            {
                // apply gravity over time if not grounded or jumping
                //  velocity += Physics.gravity * Time.deltaTime;
                velocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
            }

            characterManager.GetCharacterController.Move(velocity);
        }
    }

    public void ElevetaePlayer()
    {
        OnClimbElevate?.Invoke();
    }

    public void RefillAmmo()
    {
        OnRefillAmmo?.Invoke();
    }

    public void Reload()
    {
        OnReload?.Invoke();
    }
}
