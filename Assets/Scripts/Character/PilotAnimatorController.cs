using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PilotAnimatorController : CharacterAnimatorController
{
    public Action OnShootTriggered;

   [SerializeField] private float speedMultiplier = 1f;
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
                characterManager.GetCharacterController.Move(velocity);
            }
        }
    }

    public void Shoot()
    {
        OnShootTriggered?.Invoke();
    }
}
