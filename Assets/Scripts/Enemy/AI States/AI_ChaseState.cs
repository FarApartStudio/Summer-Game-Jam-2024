using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_ChaseState : AI_BaseState
{
    [SerializeField] float moveSpeed;

    [Header("InRange Transition")]
    [SerializeField] Transiton[] inRangeTransitons;

    [Header("During Chase Action")]
    [SerializeField] bool canPerfromActionDuringChase;
    [SerializeField] Transiton[] actionTransitons;
    [SerializeField] Vector2 actionTimeRange;

    [Header("Debug")]
    [SerializeField] bool changeState;
    [SerializeField] float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;

        timer = Random.Range(actionTimeRange.x, actionTimeRange.y);

        GetEnemyController(animator).CanMove(true);

        GetEnemyController(animator).navMeshAgent.speed = moveSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GetEnemyController(animator).canMove || GetEnemyController(animator).IsDead) return;

        if (GetEnemyController(animator).damageDetectRadar.TargetInRange())
        {
            if(!changeState)
            {
                changeState = true;

                int randomTransition = Random.Range(0, inRangeTransitons.Length);
                inRangeTransitons[randomTransition].Execute(animator);
            }
        }
        else
        {
            GetEnemyController(animator).navMeshAgent.SetDestination(GetEnemyController(animator).GetTarget.position);
            GetEnemyController(animator).transform.LookAtTargetSmooth(GetEnemyController(animator).GetTarget);

            if (!canPerfromActionDuringChase) return;

            if (timer <= 0)
            {
                if (!changeState)
                {
                    changeState = true;

                    int randomTransition = Random.Range(0, actionTransitons.Length);
                    actionTransitons[randomTransition].Execute(animator);
                }
            }
            else timer -= Time.deltaTime;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
