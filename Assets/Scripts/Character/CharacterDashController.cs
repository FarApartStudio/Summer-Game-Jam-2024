using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDashController : MonoBehaviour
{
    public Action OnDodgeStart;

    [SerializeField] private float dodgeDistance;
    [SerializeField] private float dodgeDuration;
    [SerializeField] private float dodgeCooldown;
    [SerializeField] private AnimationCurve dodgeCurve;

    [Header("References")]
    [SerializeField] private CharacterManager characterManager;

    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;

    private void Update()
    {
        HandleDodge();
    }

    private void HandleDodge()
    {
        if (InputManager.Instance.GetDodgeInput() && dodgeCooldownTimer <= 0)
        {
            OnDodge();
        }

        if (isDodging)
        {
            dodgeTimer += Time.deltaTime;
            if (dodgeTimer >= dodgeDuration)
            {
                isDodging = false;
                dodgeTimer = 0;
                dodgeCooldownTimer = dodgeCooldown;
            }
        }

        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }
    }

    public void OnDodge()
    {
        if (characterManager.IsPerformingAction) return;
        OnDodgeStart.Invoke();
        characterManager.GetAnimator.SetLayerWeight(1, 0);
        Vector2 inputDirection = characterManager.GetMovementController.move;
        int animIndex = 1;

        if (!characterManager.GetMovementController.IsSprinting)
        {
            switch (inputDirection.y)
            {
                case > 0: // forward
                    animIndex = 1;
                    break;
                case < 0: // backward
                    animIndex = 2;
                    break;
            };
        }

        characterManager.GetCharacterAnimatorController.SetFloat("DodgeIndex", animIndex);
        characterManager.GetCharacterAnimatorController.PlayTargetActionAnimation("Dodge", true, true, true);
    }
}
