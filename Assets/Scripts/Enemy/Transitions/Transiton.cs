using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trasition", menuName = "My Game/AI/ Transition")]
public class Transiton : ScriptableObject
{
    public string transitionTrigger;

    public void Execute(Animator animator)
    {
        EnemyController enemyController = animator.GetComponentInParent<EnemyController>();
        if(!enemyController.isDead) animator.SetTrigger(transitionTrigger);
    }
}
