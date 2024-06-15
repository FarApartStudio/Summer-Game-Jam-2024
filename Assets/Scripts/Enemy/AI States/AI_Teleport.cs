using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Teleport : AI_BaseState
{
    [Header("Look Option")]
    [SerializeField] bool lookAtPlayer;

    [Header("Transitions")]
    [SerializeField] Transiton inRangeTransiton;
    [SerializeField] Transiton OutRangeTransiton;

    [Header("Attack Durability")]
    [SerializeField] bool chanceToStopAttack;
    [SerializeField] bool cannotStopAttack;

    bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        GetEnemyController(animator).chanceToStopAttack = chanceToStopAttack;
        GetEnemyController(animator).cannotStopAttack = cannotStopAttack;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lookAtPlayer) GetEnemyController(animator).transform.LookAtTargetSmooth(GetEnemyController(animator).GetTarget);

        if (stateInfo.normalizedTime >= 0.9 && !changeState)
        {
            changeState = true;

            if (GetEnemyController(animator).damageDetectRadar.TargetInRange()) inRangeTransiton.Execute(animator);
            else OutRangeTransiton.Execute(animator);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).cannotStopAttack = false;
        GetEnemyController(animator).chanceToStopAttack = false;
        changeState = false;
    }
}
