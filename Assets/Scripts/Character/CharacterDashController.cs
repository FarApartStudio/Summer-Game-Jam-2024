using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDashController : MonoBehaviour
{
    public Action OnDodgeStart;

    [SerializeField] private float dodgeDuration;
    [SerializeField] private float dodgeCooldown;

    [Header("References")]
    [SerializeField] private CharacterManager characterManager;

    [Header("Debug")]
    [SerializeField] private bool isDodging;
    [SerializeField] private float dodgeTimer;
    [SerializeField] private float dodgeCooldownTimer;

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
        OnDodgeStart?.Invoke();
        //characterManager.GetAnimator.SetLayerWeight(1, 0);
        characterManager.GetCharacterAnimatorController.PlayTargetActionAnimation("Dodge", true, true, true);
    }
}
