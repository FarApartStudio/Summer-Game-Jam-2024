using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_ChargeState : AI_BaseState
{
    [SerializeField] int attackIndicatorIndex;
    [SerializeField] float moveSpeed;
    [SerializeField] Transiton exitTransitons;

    [Header("Debug")]
    [SerializeField] bool changeState;

    bool charge;
    Vector3 chargePos;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
        charge = false;

        GetEnemyController(animator).ToggleAttackIndicator(attackIndicatorIndex, true);

        GetEnemyController(animator).CanMove(true);
        GetEnemyController(animator).navMeshAgent.speed = moveSpeed;

        GetEnemyController(animator).healthController.SetInvisibility(true);

      //  GetEnemyController(animator).attackInfoManager.GetDamager<MeleeDamager>("ChargeAttack").ToggleConstantDamage(true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).canMove)
        {
            if (stateInfo.normalizedTime <= 0.6)
            {
                GetEnemyController(animator).navMeshAgent.SetDestination(GetEnemyController(animator).GetTarget.position);
            }
            else
            {
                if (!charge)
                {
                    GetEnemyController(animator).transform.LookAtTargetSmooth(GetEnemyController(animator).GetTarget);
                    chargePos = GetEnemyController(animator).GetTarget.position + (animator.transform.forward * 20);
                    GetEnemyController(animator).navMeshAgent.SetDestination(chargePos);
                    charge = true;
                }
            }
        }

        if (charge  &&  stateInfo.normalizedTime >= 0.9 && GetEnemyController(animator).navMeshAgent.remainingDistance < .1f)
        {
            if (!changeState)
            {
                changeState = true;
                exitTransitons.Execute(animator);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).ToggleAttackIndicator(attackIndicatorIndex, false);
        GetEnemyController(animator).healthController.SetInvisibility(false);

      //  GetEnemyController(animator).attackInfoManager.GetDamager<MeleeDamager>("ChargeAttack").ToggleConstantDamage(false);

        changeState = false;
        charge = false;
    }
}
