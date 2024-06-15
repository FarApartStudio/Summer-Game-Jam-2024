using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AttackState : AI_BaseState
{
    [Header("Attack Indicator Index")]
    [SerializeField] int attackIndicatorIndex;

    [Header("Invulnerability")]
    [SerializeField] bool Invulnerable;

    [Header("Look Option")]
    [SerializeField] bool lookAtPlayer;

    [Header("Follow Option")]
    [SerializeField] bool followPlayer;
    [SerializeField] float followTime;
    [SerializeField] float stopFollowAnimationTime;
    [SerializeField] float moveSpeed;

    [Header("Loop Attack")]
    [SerializeField] bool canLoop;
    [SerializeField] Vector2 loopTimeRange;

    [Header("Transitions")]
    [SerializeField] Transiton inRangeTransiton;
    [SerializeField] Transiton OutRangeTransiton;

    [Header("Attack Durability")]
    [SerializeField] bool chanceToStopAttack;
    [SerializeField] bool cannotStopAttack;
    [SerializeField] bool goingToNext;

    [Header("Debug")]
    [SerializeField] bool changeState;
    [SerializeField] bool stoppedMoving;

    float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        stoppedMoving = false;

        if(Invulnerable)GetEnemyController(animator).healthController.SetVulnerable(true);

        GetEnemyController(animator).chanceToStopAttack = chanceToStopAttack;
        GetEnemyController(animator).cannotStopAttack = cannotStopAttack;
        GetEnemyController(animator).ToggleAttackIndicator(attackIndicatorIndex, true);
        GetEnemyController(animator).SetIsAttacking(true);

        if (followPlayer)
        {
            GetEnemyController(animator).CanMove(true);
            GetEnemyController(animator).navMeshAgent.speed = moveSpeed;
        }
        else GetEnemyController(animator).CanMove(false);

        if (canLoop) timer = Random.Range(loopTimeRange.x, loopTimeRange.y);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).isDead) return;

        if (followPlayer && !GetEnemyController(animator).canMove  && stateInfo.normalizedTime <= 0.3) return;

        if (stateInfo.normalizedTime <= 0.3 || lookAtPlayer) GetEnemyController(animator).transform.LookAtTargetSmooth(GetEnemyController(animator).GetTarget);

        if(canLoop)
        {
            if (followPlayer && GetEnemyController(animator).navMeshAgent.isActiveAndEnabled)
            {
                if (GetEnemyController(animator).damageDetectRadar.TargetInRange())
                    GetEnemyController(animator).navMeshAgent.SetDestination(animator.transform.parent.position);
                else
                    GetEnemyController(animator).navMeshAgent.SetDestination(GetEnemyController(animator).GetTarget.position);
            }


            if (timer <= 0)
            {
                if (!changeState)
                {
                    changeState = true;
                    if (GetEnemyController(animator).damageDetectRadar.TargetInRange()) inRangeTransiton.Execute(animator);
                    else OutRangeTransiton.Execute(animator);
                }
            }
            else timer -= Time.deltaTime;
        }
        else
        {
            if (followPlayer && GetEnemyController(animator).canMove)
            {
                if (stateInfo.normalizedTime > followTime && !GetEnemyController(animator).damageDetectRadar.TargetInRange())
                    GetEnemyController(animator).navMeshAgent.SetDestination(GetEnemyController(animator).GetTarget.position);
            }

            if (followPlayer && !stoppedMoving && stateInfo.normalizedTime >= stopFollowAnimationTime)
            {
                stoppedMoving = true;
                GetEnemyController(animator).CanMove(false);
            }

            if (stateInfo.normalizedTime >= 0.9 && !changeState)
            {
                changeState = true;
                if (GetEnemyController(animator).damageDetectRadar.TargetInRange()) inRangeTransiton.Execute(animator);
                else OutRangeTransiton.Execute(animator);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!goingToNext)
        {
            if (Invulnerable) GetEnemyController(animator).healthController.SetVulnerable(false);
            GetEnemyController(animator).ToggleAttackIndicator(attackIndicatorIndex, false);
            GetEnemyController(animator).cannotStopAttack = false;
            GetEnemyController(animator).SetIsAttacking(false);
        }

        changeState = false;
        stoppedMoving = false;
    }
}
