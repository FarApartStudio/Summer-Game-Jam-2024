using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossBase : StateMachineBehaviour
{
    private Boss boss;

    public Boss GetBoss(Animator animator)
    {
        return boss ?? animator.GetComponentInParent<Boss>();
    }

    protected void ExecuteRandomTransition( ref bool changeState , Transiton[] transitions, Animator animator)
    {
        if (changeState) return;
        Transiton random = transitions[Random.Range(0, transitions.Length)];
        random.Execute(animator);
        changeState = true;
    }

    protected void ExecuteRandomTransition(Transiton[] transitions, Animator animator)
    {
        Transiton random = transitions[Random.Range(0, transitions.Length)];
        random.Execute(animator);
    }
}

