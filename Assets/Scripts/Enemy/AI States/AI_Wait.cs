using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Wait : AI_BaseState
{
    [SerializeField] float waitTime;
    [SerializeField] bool canMove;
    [SerializeField] Transiton[] exitTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    float currentWaitTime;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).CanMove(canMove);
        currentWaitTime = waitTime;
        changeState = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (canMove && !GetEnemyController(animator).canMove) return;

        if (currentWaitTime > 0)
        {
            currentWaitTime -= Time.deltaTime;
            return;
        }

        if (!changeState)
        {
            changeState = true;

            int randomTransition = Random.Range(0, exitTransitons.Length);
            exitTransitons[randomTransition].Execute(animator);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
