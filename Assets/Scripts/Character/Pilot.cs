using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum CameraDirection
{
    Left, Right
}


public class Pilot : CharacterManager
{
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private float sprintFOV = 70;
    [SerializeField] private float reviveInvisibilityTime = 2f;
    [SerializeField] private float normalMoveSpeed = 2.5f;
    [SerializeField] private float aimMoveSpeed = 1;
    [SerializeField] private CameraDirection currentCameraDirection;
    [SerializeField] private PilotAnimatorController pilotAnimatorController;
    [SerializeField] private CharacterCombatController characterCombatController;
    [SerializeField] private CharacterDodge characterDodge;
    [SerializeField] private HealthController healthController;
    [SerializeField] private ImpactReceiver impactReceiver;

    [Header("Effect")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ParticleSystem healParticle;

    [Header("Cam Settings")]
    [SerializeField] private Vector3 hipCameraTargetPos;
    [SerializeField] private Vector3 aimLeftCameraTargetPos;
    [SerializeField] private Vector3 aimRightCameraTargetPos;
    [SerializeField] private bool canJump = true;

    public CharacterCombatController GetCharacterCombatController => characterCombatController;
    public HealthController GetHealthController => healthController;

    private void Start()
    {
        movementController.OnSprintChange += OnSprintChange;
        pilotAnimatorController.OnShootTriggered += OnFire;

        movementController.CanSprint = () => characterCombatController.GetAimMode == ViewMode.HipFire && healthController.IsAlive;
        movementController.CanMove = () => canMove && healthController.IsAlive;
        movementController.CanRotate = () => canRotate && healthController.IsAlive;
        movementController.CanJump = () => canJump && characterCombatController.GetAimMode == ViewMode.HipFire && healthController.IsAlive;

        characterCombatController.CamAim = ()=> !IsPerformingAction && healthController.IsAlive;

        characterDodge.CanDodge = () => canMove && characterCombatController.GetAimMode == ViewMode.HipFire && healthController.IsAlive && movementController.Grounded;

        characterDodge.OnDodgeStart += () =>
        {
            healthController.SetInvisibility(true);
        };

        characterDodge.OnDodgeStop += () =>
        {
            healthController.SetInvisibility(false);
        };

        healthController.OnHit += OnHit;
        healthController.OnDie += OnDie;
        healthController.OnHeal += (amount) =>
        {
            healParticle.Play();
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
        }

        //if (InputManager.Instance.GetReloadInput())
        //{
        //    if (!healthController.IsAlive)
        //    {
        //        Revive();
        //    }
        //}

        //if (InputManager.Instance.GetSwapWeaponInput())
        //{

        //}
    }

    private void OnCameraRecoil(Vector2 vector)
    {
        cameraController.AddRecoil(vector);
    }

    public void ChangeCameraDirection()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            switch (currentCameraDirection)
            {
                case CameraDirection.Left:
                    currentCameraDirection = CameraDirection.Right;
                    CameraManager.Instance.ChangeCameraDirection(CameraDirection.Right);
                    break;
                case CameraDirection.Right:
                    currentCameraDirection = CameraDirection.Left;
                    CameraManager.Instance.ChangeCameraDirection(CameraDirection.Left);
                    break;
                default:
                    break;
            }
        }
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
                movementController.SetMoveSpeed(normalMoveSpeed);
                break;
            case ViewMode.Aim:
                CameraManager.Instance.ToggleAimCamera(true);
                movementController.SetRotateOnMove(false);
                movementController.SetMoveSpeed(aimMoveSpeed);
                break;
        }
    }

    private void OnFire()
    {
        characterCombatController.SpawnArrow();
    }

    private void OnHit(DamageInfo damageInfo)
    {
        if(damageInfo.knockback)
        {
            if (characterCombatController.GetAimMode == ViewMode.Aim)
            {
                characterCombatController.TryCancleShoot();
            }

            CameraManager.Instance.ShakeCamera(Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Rumble, .25f, .5f, damageInfo.hitDirection);

            impactReceiver.AddImpact(damageInfo.hitDirection, 50f);
            pilotAnimatorController.PlayTargetActionAnimation("Knockback", true);
        }

        hitParticle.Play();

        // pilotAnimatorController.PlayTargetActionAnimation("Hit", true);
    }

    private void OnDie(DamageInfo obj)
    {
        if (characterCombatController.GetAimMode == ViewMode.Aim)
        {
            characterCombatController.TryCancleShoot();
        }
        pilotAnimatorController.PlayTargetActionAnimation("Death", true);
        pilotAnimatorController.SetlayerWight(1, 0);
    }

    public void Revive ()
    {
        IEnumerator ReviveSequence()
        {
            healthController.SetInvisibility(true);
            yield return new WaitForSeconds(reviveInvisibilityTime);
            healthController.SetInvisibility(false);
        }
        StartCoroutine(ReviveSequence());
        healthController.RestoreHeal(healthController.GetMaxHealth);
        pilotAnimatorController.SetlayerWight(1, 1);
        pilotAnimatorController.PlayTargetActionAnimation("Revive", true);
        ToggleMovement(true);

        Debug.Log("Revive + " + healthController.GetCurrentHealth);
    }

    public void ToggleMovement (bool value)
    {
        canControl = value;
        canMove = value;
        canJump = value;
        canRotate = value;
        isPerformingAction = !value;
        movementController.StopMovement();
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
