using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AI_InvulnerabeState : AI_BaseState
{
    public enum InvulnerabeState { Start, Mid, End}

    [SerializeField] InvulnerabeState state;

    [ShowIf("state", InvulnerabeState.Mid)]
    [SerializeField] Vector2 invulnerabeTimeRange;

    [HideIf("state", InvulnerabeState.Start)]
    [SerializeField] Transiton nextTransiton;

    [ShowIf("state", InvulnerabeState.End)]
    [SerializeField] float endDuration;


    [Header("Debug")]
    [SerializeField] bool changeState;
    float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;

        switch (state)
        {
            case InvulnerabeState.Start:
                GetEnemyController(animator).CanMove(false);
                GetEnemyController(animator).healthController.SetVulnerable(true);
                break;
            case InvulnerabeState.Mid:
                timer = Random.Range(invulnerabeTimeRange.x, invulnerabeTimeRange.y);
                break;
            case InvulnerabeState.End:

                break;
            default:   break;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == InvulnerabeState.Mid)
        {
            if (timer <= 0)
            {
                if (!changeState)
                {
                    changeState = true;
                    nextTransiton.Execute(animator);
                }
            }
            else timer -= Time.deltaTime;
        }
        else if (state == InvulnerabeState.End)
        {
            if (!changeState && stateInfo.normalizedTime >= endDuration)
            {
                changeState = true;
                GetEnemyController(animator).healthController.SetVulnerable(false);
                nextTransiton.Execute(animator);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changeState = false;
    }
}
