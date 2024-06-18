using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Action<EnemyController> OnKilled;

    public AttackInfoManager attackInfoManager { get; private set; }

    public NavMeshAgent navMeshAgent { get; private set; }
    public Animator animator{ get; private set; }
    public HealthController healthController { get; private set; }
    public Transform GetTarget => target;
    public bool canAttack { get; private set; }
    public bool IsDead => !healthController.IsAlive;
    public bool isRaging { get; private set; }
    public bool canMove { get; private set; }
    public bool IsAttacking  => isAttacking;

    public DamageDefector GetDamageDefector => damageDefector;

    [Header("Enemy Properties")]
    [SerializeField] bool isPatrolling = true;
    [SerializeField] float hitAnimationDelay;
    [SerializeField] float attackRate;
    [SerializeField] int rageAmount;
    [SerializeField] float despawnDelay = 2;

    [Header("Enemy Data")]
    [SerializeField] EnemyData enemyData;
    public SkinnedMeshRenderer[] bodyParts;
    [Header("KnockBack")]
    [SerializeField] float knockBackDistance = 5;

    [Header("Raders")]
    public DetectRadar searchDetectRadar;
    public DetectRadar damageDetectRadar;

    [Header("Vfx Raders")]
    public GameObject stunVfx;

    [Header("Hit Points")]
    HitPoint[] hitPoints;

    [Header("Attack Indicators")]
    public GameObject[] attackIndicators;

    [Header("Checks")]
    public bool canPlayHit = true;
    public bool isStuned = false;
    public bool chanceToStopAttack = false;
    public bool cannotStopAttack = false;
    [SerializeField] Transform target;

    float speed;
    Vector3 lastPosition;
    float timer;
    int rageCounter;
    bool isAttacking;

    WaitForSeconds hitDelayTime;
    NavMeshObstacle navMeshObs;
    DamageDefector damageDefector;
    public EnemyData GetEnemyData() => enemyData;
    public void SwapSkin() => enemyData.skinSets.SelectRandomSkin(bodyParts);
    public void ToggleAttacking(bool newState) => canAttack = newState;
    public void ToggleStunVfx(bool newState) { stunVfx.SetActive(newState); }
    public void ToggleAttackIndicator(int index, bool newState) { attackIndicators[index].SetActive(newState); }
    public void ToggleRaging(bool newState) => isRaging = newState;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshObs = GetComponent<NavMeshObstacle>();

        healthController = GetComponent<HealthController>();

        animator = GetComponentInChildren<Animator>();

        attackInfoManager = GetComponentInChildren<AttackInfoManager>();

        damageDefector = GetComponentInChildren<DamageDefector>();

        hitPoints = GetComponentsInChildren<HitPoint>();
    }

    private void Start()
    {
        //navMeshAgent.updateRotation = false;
        hitDelayTime = new WaitForSeconds(hitAnimationDelay);
        damageDefector.OnDefect = OnDamageDefect;
        AssignEvents();
    }

    private void OnDamageDefect()
    {
        if (!healthController.IsAlive ||  isAttacking) return;
        animator.CrossFade("Dodge", 0.1f);
    }

    private void Update()
    {
        AttackTimer();
    }

    public void Activate(bool patrol)
    {
        DeSpawnArrowOnBody();
        SetUpEnemy (enemyData);
        animator.SetBool("Patrol", patrol);
    }

    public void SetUpEnemy(EnemyData enemyData)
    {
        this.enemyData = enemyData;
        healthController.enabled = true;
        navMeshObs.enabled = false;
        navMeshAgent.enabled = true;
        canMove = true;
        canPlayHit = true;
        ToggleHitPoints(true);

        healthController.SetUp(enemyData.health);

        foreach (GameObject attackIndicator in attackIndicators) attackIndicator.SetActive(false);

        attackInfoManager.SetDamagerInfo(this, enemyData.damage, 0, 0);

        SwapSkin();
    }

    public void AssignEvents()
    {
        healthController.OnHit += OnHit;
        healthController.OnDie += OnDie;

        foreach (HitPoint hitPoint in hitPoints)
        {
            hitPoint.AssignHealthSystem(healthController);
        }
    }

    private void OnHit(DamageInfo damageInfo)
    {
        //if (!canPlayHit || isStuned || cannotStopAttack) return;

        //if (chanceToStopAttack && UnityEngine.Random.Range(0, 10) > 5) return;

        //animator.SetTrigger("Hit");
        //StartCoroutine(nameof(HitDelay));

        ChestIfPatrolling();
    }

    private void ChestIfPatrolling()
    {
        if (!isPatrolling) return;
        isPatrolling = false;
        animator.CrossFade("Chase", 0.1f);
    }

    private void OnDie(DamageInfo info)
    {
        StopAllCoroutines();
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        navMeshObs.enabled = false;

        ToggleHitPoints (false);

        yield return new WaitForSecondsRealtime(.15f);

        animator.CrossFade("Death" , 0.1f);
        navMeshAgent.enabled = false;

        OnKilled?.Invoke(this);

        yield return new WaitForSecondsRealtime(despawnDelay);

        ObjectPoolManager.ReleaseObject(gameObject);
    }

    private void KnockBack(Vector3 direction)
    {
        speed = navMeshAgent.speed;
        if (enemyData.canKnockBack)
        {
            if (navMeshAgent.isActiveAndEnabled) StartCoroutine(navMeshAgent.KnockBack(-transform.forward, .2f, speed));
            else
            {
                Vector3 newPosition = transform.position + (direction * knockBackDistance);
                StartCoroutine(LerpPosition(newPosition, .2f, null, null));
            } 
        } 
    }

    public void Stun()
    {
        if(enemyData.enemyType == EnemyData.EnemyType.Boss) animator.SetTrigger("Stun");
    } 

    public void SetIsAttacking (bool status) => isAttacking = status;

    IEnumerator HitDelay()
    {
        canPlayHit = false;
        yield return hitDelayTime;
        canPlayHit = true;
    }

    public void AttackTimer()
    {
        if (canAttack) return;
        if (timer <= 0)
        {
            canAttack = true;
            timer = attackRate;
        }
        else timer -= Time.deltaTime;
    }
    
    public void CheckRageStatus()
    {
        if (!enemyData.hasRage|| isRaging) return;

        rageCounter++;

        if (rageCounter >= rageAmount)
        {
            isRaging = true;
            rageCounter = 0;
        } 
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void LookAtTarget()
    {
       transform.LookAtTargetSmooth(target.position);
    }

    public Vector3 RoamAround()
    {
        Vector3 nextDestination;

        if (EnemyAIActions.RandomPosition(transform.position, searchDetectRadar.GetScanSize(), out nextDestination))
        {
            navMeshAgent.SetDestination(nextDestination);
        }

        return nextDestination;
    }

    public void TeleportToRandomPosition()
    {
        Vector3 nextDestination;

        if (EnemyAIActions.RandomPosition(GetTarget.position, damageDetectRadar.GetScanSize(), out nextDestination))
        {
            navMeshAgent.Warp(nextDestination);
        }
    }

    public void CanMove(bool status)
    {
        if (canMove && status  == true) return; 
        canMove = false;

        if (status)
        {
            StartCoroutine(nameof( EnableMovement));
        }
        else
        {
            navMeshAgent.enabled = false;
            navMeshObs.enabled = true;
        }
    }

    public IEnumerator EnableMovement()
    {
        navMeshObs.enabled = false;
        yield return new WaitForSecondsRealtime(0.05f);
        navMeshAgent.enabled = true;
        canMove = true;
    }

    private void DeSpawnArrowOnBody()
    {
        Arrow[] arrows = GetComponentsInChildren<Arrow>();
        foreach (Arrow arrow in arrows)
        {
            arrow.ForceDeSpawn();
        }
    }

    void ToggleHitPoints (bool state)
    {
        foreach (HitPoint hitBox in hitPoints)
        {
            hitBox.ToggleActive(state);
        }
    }

    void SetRigidBodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = state;
        }
    }

    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.isTrigger = state;
        }
    }

    public IEnumerator LerpPosition(Vector3 targetPosition, float duration, Action OnStart, Action OnFinish)
    {
        if (OnStart != null) OnStart.Invoke();
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        yield return null;
        if (OnFinish != null) OnFinish.Invoke();
    }
}
