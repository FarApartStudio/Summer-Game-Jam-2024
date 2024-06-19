using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossAttack : AI_BossBase
{
    [Header("Transitions")]
    [SerializeField] Transiton[] inRangeTransitons;
    [SerializeField] Transiton[] outRangeTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        Vector3 previousPos = GetBoss(animator).transform.position;
        Vector3 firstDestination = GetBoss(animator).GetTarget.position + (GetBoss(animator).transform.forward * 10);
        GetBoss(animator).GoToTarget(GetBoss(animator).GetTarget.position, stateInfo.length / 2,
            ()=>
            {
                GetBoss(animator).GoToTarget(previousPos, stateInfo.length / 2);
            });
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!changeState)
        {
            changeState = true;

        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
