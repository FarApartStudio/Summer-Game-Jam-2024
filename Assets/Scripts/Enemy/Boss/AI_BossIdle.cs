using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossIdle : AI_BossBase
{
    [Header("Transitions")]
    [SerializeField] Transiton[] inRangeTransitons;
    [SerializeField] Transiton[] outRangeTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
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
