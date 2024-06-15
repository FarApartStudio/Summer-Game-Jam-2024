using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AI_Dodge : AI_BaseState
{
    [SerializeField] float moveRange;
    [SerializeField] float moveForce;
    [SerializeField] Transiton[] inRangeTransitons;
    [SerializeField] Transiton OutRangeTransiton;
    [SerializeField] float normalisedAnimationExitTime;

    [Header("Debug")]
    [SerializeField] bool changeState;

    Vector3 targetPosition;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        GetEnemyController(animator).CanMove(false);

        Vector3 damagePosition = GetEnemyController(animator).GetDamageDefector.GetLastDefectPosition();
        Vector3 direction = (damagePosition - GetEnemyController(animator).transform.position).normalized;
        direction.y = 0;
        Vector3 sideDirection = Vector3.Cross(Vector3.up, direction).normalized;

        if (Random.value > 0.5f)
        {
            sideDirection = -sideDirection;
        }

        targetPosition = GetEnemyController(animator).transform.position + sideDirection * moveRange;

        if (!EnemyAIActions.IsPositionValid(targetPosition, out Vector3 final))
        {
            targetPosition = GetEnemyController(animator).transform.position;
        }

       // GetEnemyController(animator).StartCoroutine(GetEnemyController(animator).navMeshAgent.KnockBack(targetPosition, 0.2f, 5, moveForce));

        GetEnemyController(animator).StartCoroutine(GetEnemyController(animator).LerpPosition (targetPosition, .2F, null, null));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).isDead) return;

        if (stateInfo.normalizedTime >= normalisedAnimationExitTime && !changeState)
        {
            changeState = true;

            if (GetEnemyController(animator).damageDetectRadar.TargetInRange())
            {
                int randomTransition = Random.Range(0, inRangeTransitons.Length);
                inRangeTransitons[randomTransition].Execute(animator);
            }
            else OutRangeTransiton.Execute(animator);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
