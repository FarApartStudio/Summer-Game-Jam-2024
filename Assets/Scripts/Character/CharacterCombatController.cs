using Pelumi.ObjectPool;
using Pelumi.SurfaceSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum ViewMode
{
    HipFire,
    Aim
}

public class CharacterCombatController : MonoBehaviour
{

    public enum WeaponState
    {
        Idle,
        Shooting,
        Reloading
    }

    public Func<bool> CanFire;
    public Func<bool> CamAim;

    public event Action<Vector2> OnCameraRecoil;
    public event Action OnFire;
    public event Action<float> OnAimAccuracyChanged;
    public event Action<float> OnAmmoUpdate;
    public event Action OnSuccessfulHit;

    public event Action OnReloadStart;
    public event Action<float> OnReloading;
    public event Action OnReloadEnd;

    public event Action<bool, Vector3> HitablePointDetected;
    public event Action hitDetected;
    public event Action<ViewMode> OnAimModeChanged;

    [Header("References")]
    [SerializeField] private PilotAnimatorController pilotAnimatorController;
    [SerializeField] private Transform aimTargetDebug;

    [Header("Weapon")]
    [SerializeField] private Transform rightHandSocket;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private LayerMask aimHelperLayerMask;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private Bow currentBow;

    [Header("WeaponToEdit")]
    [SerializeField] private Arrow arrow;
    [MinMaxSlider(0, 100, true)][SerializeField] private Vector2 _damageRange;
    [SerializeField] private float maxRecoil = 5;
    [SerializeField] private float stabilityDuration = 2f;
    [SerializeField] private float arrowSpeed = 25;
    [SerializeField] private float fireRate = 0.1f;

    [Header("Aiming")]
    [SerializeField] private float aimRotateSpeed = 10;
    [SerializeField] private float aimResetDelay = 0.5f;
    [SerializeField] private Rig aimingRig;
    [SerializeField] private float aimingRigLerpSpeed = 20;
    [SerializeField] private float minDetectRange = 20f;

    [Header("Debug Gun Controls")]
    [SerializeField] private bool autoHideMouse = false;
    [SerializeField] private bool isTriggerHeld = false;
    [SerializeField] private bool isTriggerReleased = true;
    [SerializeField] private float currentTriggerHoldTime;
    [SerializeField] private float currentFireRate;

    private ViewMode aimMode = ViewMode.HipFire;
    private WeaponState weaponState = WeaponState.Idle;
    private Camera cam;
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
    private bool cancelShot = false;
    private float lastHoldTime = 0;

    public ViewMode GetAimMode => aimMode;
    public WeaponState GetWeaponState => weaponState;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleAnimationWeights();
        HandleInput();
        HandleMouseWorldPosition();
        HandleRotationWithCamera();
    }

    public void HandleAnimationWeights()
    {
        if (aimInput && IsFacingTarget())
        {
            shootingRigWeight = 1;
        }
        else
        {
            shootingRigWeight = 0;
        }

        aimingRig.weight = Mathf.Lerp(aimingRig.weight, shootingRigWeight, Time.deltaTime * aimingRigLerpSpeed);
        pilotAnimatorController.SetBool("InAction", aimInput);
        pilotAnimatorController.Animator.SetFloat("Turn", turnValue, .2f, Time.deltaTime);
    }

    public void HandleInput()
    {
        SetTriggerHeld(InputManager.Instance.GetAttackInputAction().IsPressed());

        if (InputManager.Instance.GetAimInput())
        {
            TryCancleShoot();
        }
    }

    public void SetTriggerHeld(bool trigger)
    {
        bool fireRateReady = FireRateReady();

        if (trigger && !fireRateReady)
            return;

        if (trigger && !CamAim()) 
            return;

        if (trigger && cancelShot)
            return;

        if (isTriggerHeld)
        {
            currentTriggerHoldTime += Time.deltaTime;
            OnAimAccuracyChanged?.Invoke(GetCurrentAimAccuracy());
            currentBow.SetAccuracy(GetCurrentAimAccuracy());
        }

        if (isTriggerHeld && !trigger)
        {
            lastHoldTime = currentTriggerHoldTime;
            currentTriggerHoldTime = 0;
            isTriggerReleased = true;
            currentFireRate = fireRate;
            cancelShot = false;
        }

        isTriggerHeld = trigger;

        if (trigger)
        {
            HandleAim(true);
        }
        else
        {
            HandleAim(false);
        }
    }

    public void TryCancleShoot()
    {
        if (aimMode != ViewMode.Aim) 
            return;

        cancelShot = true;
        pilotAnimatorController.PlayAnimation(1, "Default", 0.2f);
        HandleAim(false);
    }

    public void HandleAim(bool aim)
    {
        if (aim == aimInput) return;

        aimInput = aim;
        pilotAnimatorController.SetBool("IsAiming", aimInput);
        ChangeAimMode(aimInput ? ViewMode.Aim : ViewMode.HipFire);
    }

    public void SpawnArrow ()
    {
        CameraManager.Instance.ShakeCamera(Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Rumble, .25f, .5f);


        Vector3 finalDestination = GetBulletSpread(targetDetectPos, maxRecoil, GetLastAimAccuracy());

        shootDirection = finalDestination - currentBow.GetFirePoint().position;
        shootDirection.Normalize();
        Arrow arrowInstance = ObjectPoolManager.SpawnObject(arrow, currentBow.GetFirePoint().position,transform.rotation);
        arrowInstance.OnHit = HitDetection;
        arrowInstance.Init(shootDirection, arrowSpeed);
        currentBow.OnFire();
    }

    public void HitDetection(ArrowHit arrowHit)
    {
        // Check if we hit a surface
        if (arrowHit.Collider.gameObject.TryGetComponent<Surface>(out Surface surface))
        {
            GameObject impactEffect = ObjectPoolManager.SpawnObject(surface.GetSurfaceInfo()._particleSystem);
            impactEffect.transform.position = arrowHit.HitPoint + arrowHit.HitNormal * 0.01f;
            impactEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, arrowHit.HitNormal);
            impactEffect.gameObject.SetActive(true);
            impactEffect.transform.LookAt(arrowHit.HitPoint + arrowHit.HitNormal);
        }

        if (arrowHit.Collider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = CalculateDamage();
            damageInfo.hitDirection = arrowHit.HitDirection;
            damageInfo.damageType = DamageType.Projectile;
            damageInfo.critical = false;
            damageInfo.knockback = false;
            damageable.Damage(damageInfo, arrowHit.HitPoint);

            OnSuccessfulHit?.Invoke();
        }
    }

    public void ChangeAimMode(ViewMode mode)
    {
        aimMode = mode;
        currentBow.SetState(mode == ViewMode.Aim ? Bow.State.Pulling : Bow.State.Normal);
        OnAimModeChanged?.Invoke(aimMode);
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
        Ray ray = cam.ScreenPointToRay(screeCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit screeCenterHit, float.MaxValue, detectLayer))
        {
            mouseWorldPosition = Vector3.Lerp (mouseWorldPosition , IsTooClose(screeCenterHit.point) ? ray.GetPoint(100) : screeCenterHit.point, Time.deltaTime * 20);
            //mouseWorldPosition = IsTooClose (screeCenterHit.point) ? ray.GetPoint(1000) : screeCenterHit.point;
        }
        else
        {
            // mouseWorldPosition = cam.transform.position + cam.transform.forward * 1000;
            mouseWorldPosition = ray.GetPoint(1000);
        }

        Debug.DrawLine(currentBow.GetFirePoint().position, mouseWorldPosition, Color.red, 1f);

        if (Physics.Linecast(currentBow.GetFirePoint().position, mouseWorldPosition, out RaycastHit linecastHit, detectLayer)
            && linecastHit.point != mouseWorldPosition
            )
        {
            targetDetectPos = linecastHit.point;

            Vector2 screenPosition = cam.WorldToScreenPoint(targetDetectPos);
            HitablePointDetected?.Invoke(true, screenPosition);
        }
        else
        {
            targetDetectPos = mouseWorldPosition;
            HitablePointDetected?.Invoke(false, targetDetectPos);
        }

        aimTargetDebug.position = Vector3.Lerp(aimTargetDebug.position, mouseWorldPosition, Time.deltaTime * 10);
    }

    public void HandleRotationWithCamera()
    {
        if (isTriggerHeld || aimMode == ViewMode.Aim)
        {
            previousFoward = transform.forward;

            Vector3 worldAimTarget = mouseWorldPosition;

            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimRotateSpeed);

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

    public bool IsTooClose(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < minDetectRange;
    }   

    public int CalculateDamage()
    {
        return (int)Mathf.Lerp(_damageRange.x, _damageRange.y, GetLastAimAccuracy());
    }

    public float GetLastAimAccuracy() => Mathf.Clamp(lastHoldTime / stabilityDuration, 0, 1);
    public float GetCurrentAimAccuracy() => Mathf.Clamp(currentTriggerHoldTime / stabilityDuration, 0, 1);

    public Vector3 GetBulletSpread(Vector3 screenCenterPoint,float spreadRange, float normalisedStability)
    {
        float normalsiedXSpread = Mathf.Lerp(spreadRange, 0, normalisedStability);
        float normalsiedYSpread = Mathf.Lerp(spreadRange,0, normalisedStability);

        screenCenterPoint.x += UnityEngine.Random.Range(-normalsiedXSpread, normalsiedXSpread);
        screenCenterPoint.y += UnityEngine.Random.Range(-normalsiedYSpread, normalsiedYSpread);

        return screenCenterPoint;
    }

    public bool IsFacingTarget()
    {
        Vector3 direction = (targetDetectPos - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, direction);
        return dot > 0.7f;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDetectRange);
    }
}
