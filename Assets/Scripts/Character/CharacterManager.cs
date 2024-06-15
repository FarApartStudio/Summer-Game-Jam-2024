using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform cameraTargetTransfrom;
    [SerializeField] protected CharacterAnimatorController characterAnimatorController;
    [SerializeField] protected CharacterController characterController;
    [SerializeField] protected MovementController movementController;
    [SerializeField] protected CameraController cameraController;
    [SerializeField] protected Animator animator;

    [Header("Debug")]
    [SerializeField] protected bool canControl = true;
    public bool CanControl => canControl;

    public MovementController GetMovementController => movementController;
    public CameraController GetCameraController => cameraController;
    public Animator GetAnimator => animator;
    public CharacterController GetCharacterController => characterController;
    public CharacterAnimatorController GetCharacterAnimatorController => characterAnimatorController;

    [Header("Character Flags")]
    [SerializeField] protected bool isPerformingAction;
    [SerializeField] protected bool canRotate;
    [SerializeField] protected bool canMove;
    [SerializeField] protected bool applyRootMotion;
    [SerializeField] protected bool applyRootGravity;
    [SerializeField] protected bool showWeapon;

    public bool IsPerformingAction => isPerformingAction;
    public bool CanRotate => canRotate;
    public bool CanMove => canMove;
    public bool ApplyRootMotion => applyRootMotion;

    public bool ApplyRootGravity => applyRootGravity;

    public void SetApplyRootMotion(bool state)
    {
        applyRootMotion = state;
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
    }

    public void SetCanRotate(bool state)
    {
        canRotate = state;
    }

    public void SetIsPerformingAction(bool state)
    {
        isPerformingAction = state;
    }

    public void SetApplyRootGravity(bool state)
    {
        applyRootGravity = state;
    }

    public virtual void ResetActions()
    {
       isPerformingAction = false;
       canRotate = true;
       canMove = true;
       applyRootMotion = false;
       applyRootGravity = false;
    }

    public virtual void SetShowWeapon(bool show)
    {
        showWeapon = show;
    }
}
