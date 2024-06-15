using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_StunState : AI_BaseState
{
    [SerializeField] Transiton exitTransiton;
    [SerializeField] Vector2 timeRange;
    float timer;

    [Header("Debug")]
    [SerializeField]bool changeState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;

        GetEnemyController(animator).isStuned = true;

        timer = Random.Range(timeRange.x, timeRange.y);
        GetEnemyController(animator).ToggleStunVfx(true);

        GetEnemyController(animator).CanMove(false);
    }

   override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GetEnemyController(animator).IsDead) return;

        if (timer <= 0)
        {
            if (!changeState)
            {
                changeState = true;
                exitTransiton.Execute(animator);
            }
        }
        else  timer -= Time.deltaTime;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetEnemyController(animator).isStuned = false;
        GetEnemyController(animator).ToggleStunVfx(false);
        changeState = false;
    }
}
