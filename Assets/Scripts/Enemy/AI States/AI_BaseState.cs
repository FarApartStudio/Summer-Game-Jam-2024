using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BaseState : StateMachineBehaviour
{
    private EnemyController enemyController;

    public EnemyController GetEnemyController(Animator animator)
    {
        return enemyController ?? animator.GetComponentInParent<EnemyController>();
    }
}
