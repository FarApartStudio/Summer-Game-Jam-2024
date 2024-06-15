using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public AttackInfoManager attackInfoManager { get; private set; }
    //public Collider col { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    public Animator animator{ get; private set; }
    public HealthController healthController { get; private set; }
    public Transform GetTarget => target;
    public bool canAttack { get; private set; }
    public bool isDead { get; private set; }
    public bool isRaging { get; private set; }
    public bool canMove { get; private set; }
    public bool IsAttacking  => isAttacking;

    public DamageDefector GetDamageDefector => damageDefector;

    [Header("Enemy Properties")]
    [SerializeField] float hitDelay;
    [SerializeField] float attackRate;
    [SerializeField] int rageAmount;

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
    //EnemyManager enemyManager

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshObs = GetComponent<NavMeshObstacle>();

       // col = GetComponent<Collider>();

        healthController = GetComponent<HealthController>();

        animator = GetComponentInChildren<Animator>();

        attackInfoManager = GetComponentInChildren<AttackInfoManager>();

        damageDefector = GetComponentInChildren<DamageDefector>();

       // enemyManager = EnemyManager.Instance;
    }

    private void Start()
    {
       // target = Player.Instance.transform;

        navMeshAgent.updateRotation = false;

        hitDelayTime = new WaitForSeconds(hitDelay);
        damageDefector.OnDefect = OnDamageDefect;
        AssignEvents();
    }

    private void OnDamageDefect()
    {
        if (isAttacking) return;
        animator.CrossFade("Dodge", 0.1f);
    }

    private void OnEnable()
    {
        isDead = false;
        //col.enabled = true;
        healthController.enabled = true;
        navMeshObs.enabled = false;
        navMeshAgent.enabled = true;
        canMove = true;
        canPlayHit = true;
    }

    private void Update()
    {
        AttackTimer();
    }

    public void SetUpEnemy(int health, int damage)
    {
        healthController.SetUp(health);

        foreach (GameObject attackIndicator in attackIndicators) attackIndicator.SetActive(false);

        attackInfoManager.SetDamagerInfo(this, damage, 0, 0);

        SwapSkin();
    }

    public void AssignEvents()
    {
        //healthController.OnReceiveDamage.AddListener(DamageReceived);
        //healthController.OnHit.AddListener(Hit);
        //healthController.OnHeal.AddListener(Heal);
        //healthController.OnDied.AddListener(Die);
    }

    private void DamageReceived(DamageInfo damageInfo)
    {
      //  UIManager.Instance.SetEnemyHealth(enemyData.enemyName, damageable.GetHealthPercent());

        CheckRageStatus();
        if (damageInfo.knockback) KnockBack(damageInfo.hitDirection);
        if (damageInfo.stun) Stun();
    }

    private void Hit()
    {
        if (!canPlayHit || isStuned || cannotStopAttack) return;

        if (chanceToStopAttack && UnityEngine.Random.Range(0, 10) > 5) return;

        animator.SetTrigger("Hit");
        StartCoroutine(nameof(HitDelay));
    }

    private void Heal()
    {
      //  UIManager.Instance.SetEnemyHealth(enemyData.enemyName, damageable.GetHealthPercent());
    }

    private void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DeathSequence());    
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;

        navMeshObs.enabled = false;

        yield return new WaitForSecondsRealtime(.15f);

        animator.SetTrigger("Die");
       // dissolveMat.StartDissolve();

        //col.enabled = false;
       // damageable.enabled = false;
        navMeshAgent.enabled = false;

       // if (enemyManager != null) enemyManager.RemoveEnemy(this);
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

        if (EnemyAIActions.RandomPosition(transform.position, damageDetectRadar.GetScanSize(), out nextDestination))
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
            navMeshObs.enabled = false;
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
        yield return new WaitForSecondsRealtime(0.1f);
        navMeshAgent.enabled = true;
        canMove = true;
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

    public EnemyData GetEnemyData() => enemyData;
    public void SwapSkin() => enemyData.skinSets.SelectRandomSkin(bodyParts);
    public void ToggleAttacking(bool newState) => canAttack = newState;
    public void ToggleStunVfx(bool newState){ stunVfx.SetActive(newState);}
    public void ToggleAttackIndicator(int index, bool newState) { attackIndicators[index].SetActive(newState); }
    public void ToggleRaging(bool newState) => isRaging = newState;
}
