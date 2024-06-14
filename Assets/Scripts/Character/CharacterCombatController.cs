using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterCombatController : MonoBehaviour
{
    public enum AimMode
    {
        HipFire,
        Aim
    }

    public enum WeaponState
    {
        Idle,
        Shooting,
        Reloading
    }

    public event Func<bool> CanFire;
    public event Func<bool> CamAim;
    public event Action<Vector2> OnCameraRecoil;
    public event Action OnFire;
    public event Action<float> ModifyCrosshair;
    public event Action<float> OnAmmoUpdate;

    public event Action OnReloadStart;
    public event Action<float> OnReloading;
    public event Action OnReloadEnd;

    public event Action<bool, Vector3> GunPointDetected;
    public event Action hitDetected;
    public event Action<AimMode> OnAimModeChanged;

    [Header("References")]
    [SerializeField] private PilotAnimatorController pilotAnimatorController;
    [SerializeField] private Transform aimTargetDebug;

    [Header("Weapon")]
    [SerializeField] private Transform rightHandSocket;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform firePos;
    [SerializeField] private LayerMask hitLayer;

    [Header("ShootingRig")]
    [SerializeField] private Rig aimingRig;
    [SerializeField] private float aimingRigLerpSpeed = 20;

    [Header("Debug Gun Controls")]
    [SerializeField] private bool autoHideMouse = false;
    [SerializeField] private bool isTriggerHeld = false;
    [SerializeField] private bool isTriggerReleased = true;
    [SerializeField] private int currentBulletInBurst;
    [SerializeField] private bool CanBurstFire = false;
    [SerializeField] private float currentFireRate;
    [SerializeField] private float currentTriggerHoldTime;

    private AimMode aimMode = AimMode.HipFire;
    private WeaponState weaponState = WeaponState.Idle;
    private Camera cam;
    //[SerializeField] private WeaponModel currentWeaponModel;
    private bool aimInput = false;
    private Vector2 screeCenterPoint;
    private Vector3 mouseWorldPosition;
    private Vector3 targetDetectPos;
    private Vector3 shootDirection;
    private Vector3 previousFoward;
    private float turnValue = 0f;
    private float shootingRigWeight = 0f;
    private float turnThreshold = 0.002f;
    private bool reloaded = false;

    public AimMode GetAimMode => aimMode;
    public WeaponState GetWeaponState => weaponState;

    private void Update()
    {
        // pilotAnimatorController.SetBool("InAction", isTriggerHeld || aimInput);
        // pilotAnimatorController.Animator.SetFloat("Turn", turnValue, .2f, Time.deltaTime);

        HandleInput();
    }

    public void ChangeAimMode(AimMode mode)
    {
        aimMode = mode;
        OnAimModeChanged?.Invoke(aimMode);
    }

    public void HandleInput()
    {
        SetTriggerHeld(InputManager.Instance.GetAttackInputAction().IsPressed());

        if (InputManager.Instance.GetAimInput())
        {
            if (CamAim != null && !CamAim()) return;
            HandleAim(true);
        }
        else
        {
            HandleAim(false);
        }

        if (InputManager.Instance.GetReloadnput())
        {
            
        }

        if (InputManager.Instance.GetSwapWeaponInput())
        {
 
        }
    }

    public void SetTriggerHeld(bool trigger)
    {
        if (isTriggerHeld && !trigger)
        {
            currentTriggerHoldTime = 0;
            ModifyCrosshair?.Invoke(0);
            isTriggerReleased = true;
        }

        isTriggerHeld = trigger;
    }

    public void HandleAim(bool aim)
    {
        if (aim == aimInput) return;

        aimInput = aim;
        pilotAnimatorController.SetBool("IsAiming", aimInput);
        ChangeAimMode(aimInput ? AimMode.Aim : AimMode.HipFire);

        if (aimInput)
        {
            if (weaponState != WeaponState.Reloading)
            {
                shootingRigWeight = 1;
            }
        }
        else
        {
            shootingRigWeight = 0;

            if (weaponState != WeaponState.Reloading)
            {
                ModifyCrosshair?.Invoke(0);
            }
        }
    }

    public bool FireRateReady()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
            return false;
        }
        return true;
    }

    public void HandleMouseWorldPosition()
    {
        screeCenterPoint = new Vector2(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screeCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit screeCenterHit, float.MaxValue, hitLayer))
        {
            mouseWorldPosition = screeCenterHit.point;
        }
        else
        {
            // mouseWorldPosition = cam.transform.position + cam.transform.forward * 1000;
            mouseWorldPosition = ray.GetPoint(1000);
        }

        Debug.DrawLine(firePos.position, mouseWorldPosition, Color.red, 1f);

        if (Physics.Linecast(firePos.position, mouseWorldPosition, out RaycastHit linecastHit, hitLayer)
            && linecastHit.point != mouseWorldPosition
            )
        {
            targetDetectPos = linecastHit.point;

            Vector2 screenPosition = cam.WorldToScreenPoint(targetDetectPos);
            GunPointDetected?.Invoke(true, screenPosition);
        }
        else
        {
            targetDetectPos = mouseWorldPosition;
            GunPointDetected?.Invoke(false, targetDetectPos);
        }

        aimTargetDebug.position = Vector3.Lerp(aimTargetDebug.position, mouseWorldPosition, Time.deltaTime * 10);
    }

    public void HandleRotationWithCamera()
    {
        if (isTriggerHeld || aimMode == AimMode.Aim)
        {
            previousFoward = transform.forward;

            Vector3 worldAimTarget = mouseWorldPosition;

            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 10);

            if (Vector3.Distance(transform.forward, previousFoward) < turnThreshold)
            {
                turnValue = Mathf.Lerp(turnValue, 0, Time.deltaTime * 20);
            }
            else
            {
                float crossProduct = Vector3.Cross(transform.forward, aimDirection).y;
                turnValue = Mathf.Lerp(turnValue, crossProduct > 0 ? 1 : -1, Time.deltaTime * 20);
            }
        }
    }
}
