using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraDirection
{
    Left, Right
}


public class Pilot : CharacterManager
{
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private float sprintFOV = 70;
    [SerializeField] private CameraDirection currentCameraDirection;
    [SerializeField] private PilotAnimatorController pilotAnimatorController;
    [SerializeField] private CharacterCombatController characterCombatController;
    [SerializeField] private CharacterDodge characterDodge;
    [SerializeField] private HealthController healthController;

    [SerializeField] private Vector3 hipCameraTargetPos;
    [SerializeField] private Vector3 aimLeftCameraTargetPos;
    [SerializeField] private Vector3 aimRightCameraTargetPos;
    [SerializeField] private bool canJump = true;

    public CharacterCombatController GetCharacterCombatController => characterCombatController;

    private void Start()
    {
        movementController.OnSprintChange += OnSprintChange;
        pilotAnimatorController.OnShootTriggered += OnFire;

        movementController.CanSprint = () => characterCombatController.GetAimMode == ViewMode.HipFire;
        movementController.CanMove = () => canMove;
        movementController.CanRotate = () => canRotate;

        characterCombatController.CamAim = ()=> !IsPerformingAction;

        characterDodge.CanDodge = () => characterCombatController.GetAimMode == ViewMode.HipFire;

        characterDodge.OnDodgeStart += () =>
        {
            healthController.SetInvisibility(true);
        };

        characterDodge.OnDodgeStop += () =>
        {
            healthController.SetInvisibility(false);
        };

        ResetActions();

        CameraManager.Instance.Follow (cameraTargetTransfrom);
    }

    private void Update()
    {
        if (canControl)
        {
            movementController.OnMove(InputManager.Instance.GetMovementInput());

            movementController.OnSprint(InputManager.Instance.GetSprintInput());

            if (canJump) movementController.OnJump(InputManager.Instance.GetJumpInput());

            cameraController.UpdateInput(InputManager.Instance.GetCameraInput());

            characterCombatController.OnAimModeChanged += OnAimModeChanged;

            if (Input.GetKeyDown(KeyCode.T))
            {
                //switch (currentCameraDirection)
                //{
                //    case CameraDirection.Left:
                //        ChangeCameraDirection(CameraDirection.Right);
                //        break;
                //    case CameraDirection.Right:
                //        ChangeCameraDirection(CameraDirection.Left);
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        if (InputManager.Instance.GetReloadInput())
        {

        }

        if (InputManager.Instance.GetSwapWeaponInput())
        {

        }
    }

    private void OnCameraRecoil(Vector2 vector)
    {
        cameraController.AddRecoil(vector);
    }

    public void ChangeCameraDirection(CameraDirection direction)
    {
        currentCameraDirection = direction;
        CameraManager.Instance.ChangeCameraDirection(currentCameraDirection);
    }

    private void OnSprintChange(bool state)
    {
        switch (state)
        {
            case true:
                CameraManager.Instance.ChangeFollowFOV(sprintFOV, .5f);
                break;
            case false:
                CameraManager.Instance.ChangeFollowFOV(normalFOV, .5f);
                break;
        }
    }

    private void OnAimModeChanged(ViewMode mode)
    {
        movementController.StopSprint();

        switch (mode)
        {
            case ViewMode.HipFire:
                CameraManager.Instance.ToggleAimCamera(false);
                movementController.SetRotateOnMove(true);
               // isPerformingAction = false;
                break;
            case ViewMode.Aim:
                CameraManager.Instance.ToggleAimCamera(true);
                movementController.SetRotateOnMove(false);
               // isPerformingAction = true;
                break;
        }
    }

    private void OnFire()
    {
        characterCombatController.SpawnArrow();
    }

    public override void SetShowWeapon(bool show)
    {
        base.SetShowWeapon(show);
    }

    public override void ResetActions()
    {
        base.ResetActions();
    }
}
