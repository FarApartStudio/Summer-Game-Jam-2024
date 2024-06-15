using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SearchForTarget : AI_BaseState
{
    [Header("Normal Transition")]
    [SerializeField] Transiton[] inRangeTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).searchDetectRadar.TargetInRange())
        {
            if (!changeState)
            {
                changeState = true;

                int randomTransition = Random.Range(0, inRangeTransitons.Length);
                inRangeTransitons[randomTransition].Execute(animator);
            }
        }
 
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
