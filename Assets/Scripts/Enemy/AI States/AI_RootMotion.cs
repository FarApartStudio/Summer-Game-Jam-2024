using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_RootMotion : AI_BaseState
{
    [SerializeField] bool disableMovementIfInRange;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).navMeshAgent.isActiveAndEnabled)
        GetEnemyController(animator).navMeshAgent.updatePosition = false;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GetEnemyController(animator).navMeshAgent.isActiveAndEnabled) return;

        if (disableMovementIfInRange && GetEnemyController(animator).detectRadar.TargetInRange()) return;

        Vector3 position = animator.rootPosition;
        position.y = GetEnemyController(animator).navMeshAgent.nextPosition.y;

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(position, out hit, .5f, NavMesh.AllAreas);

        if(isValid)
        {
            animator.transform.parent.position = hit.position;
            GetEnemyController(animator).navMeshAgent.nextPosition = animator.transform.position;
        }
    }
}
