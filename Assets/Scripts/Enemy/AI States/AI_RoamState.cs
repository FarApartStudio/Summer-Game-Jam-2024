using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_RoamState : AI_BaseState
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transiton reachRoamExit;

    [Header("Debug")]
    [SerializeField] bool changeState;

    Vector3 roamPos;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).CanMove(true);
        GetEnemyController(animator).navMeshAgent.speed = moveSpeed;
        roamPos = GetEnemyController(animator).RoamAround();
        changeState = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GetEnemyController(animator).canMove) return;

        GetEnemyController(animator).transform.LookAtTargetSmooth(roamPos);

        //Debug.Log("Roam State" + GetEnemyController(animator).navMeshAgent.remainingDistance);

        if (GetEnemyController(animator).navMeshAgent.remainingDistance < 1f)
        {
            if (!changeState)
            {
                changeState = true;
                reachRoamExit.Execute(animator);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
