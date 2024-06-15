using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_ChangeState : AI_BaseState
{
    [SerializeField] Transiton[] inRangeTransitons;
    [SerializeField] Transiton OutRangeTransiton;
    [SerializeField] float duration;

    [Header("Debug")]
    [SerializeField] bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        GetEnemyController(animator).CanMove(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).IsDead) return;

        if (stateInfo.normalizedTime >= duration && !changeState)
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
