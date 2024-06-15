using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_IdleState : AI_BaseState
{
    [Header("Normal Transition")]
    [SerializeField] Transiton[] inRangeTransitons;
    [SerializeField] Transiton outRangeTransitons;

    [Header("Rage Transition")]
    [SerializeField] Transiton[] rageTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        GetEnemyController(animator).CanMove(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).transform.LookAtTargetSmooth(GetEnemyController(animator).GetTarget);

        if (GetEnemyController(animator).damageDetectRadar.TargetInRange())
        {
            if (!changeState && GetEnemyController(animator).canAttack)
            {
                changeState = true;

                GetEnemyController(animator).ToggleAttacking(false);

                if (GetEnemyController(animator).isRaging)
                {
                    GetEnemyController(animator).cannotStopAttack = true;
                    GetEnemyController(animator).ToggleRaging(false);
                    int randomTransition = Random.Range(0, rageTransitons.Length);
                    rageTransitons[randomTransition].Execute(animator);
                }
                else
                {
                    int randomTransition = Random.Range(0, inRangeTransitons.Length);
                    inRangeTransitons[randomTransition].Execute(animator);
                }
            }
        }
        else
        {
            if (!changeState)
            {
                changeState = true;
                outRangeTransitons.Execute(animator);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
