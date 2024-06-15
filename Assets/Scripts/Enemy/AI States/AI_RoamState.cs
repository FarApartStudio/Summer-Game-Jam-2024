using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_RoamState : AI_BaseState
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transiton exitTransitons;
    [SerializeField] float chargeDelay;

    [Header("Debug")]
    [SerializeField] bool changeState;

    Vector3 roamPos;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Roam State");
        GetEnemyController(animator).CanMove(true);
        GetEnemyController(animator).navMeshAgent.speed = moveSpeed;
        GetEnemyController(animator).RoamAround();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).navMeshAgent.remainingDistance < .1f)
        {
            GetEnemyController(animator).RoamAround();
        }

        Debug.Log("Roam State Update");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
