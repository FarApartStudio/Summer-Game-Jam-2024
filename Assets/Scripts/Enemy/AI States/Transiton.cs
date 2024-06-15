using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Transiton
{
    public string animationName;
    public float transitionDuration = 0.2f;

    public void Execute(Animator animator)
    {
        EnemyController enemyController = animator.GetComponentInParent<EnemyController>();
        if(!enemyController.IsDead) animator.CrossFade(animationName, transitionDuration);
    }
}
