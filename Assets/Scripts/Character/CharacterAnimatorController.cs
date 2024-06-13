using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class CharacterAnimatorController : MonoBehaviour
{
    [SerializeField] protected CharacterManager characterManager;
    [SerializeField] protected MovementController movementController;
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioClip LandingAudioClip;
    [SerializeField] protected AudioClip[] FootstepAudioClips;
    [Range(0, 1)][SerializeField] protected float FootstepAudioVolume = 0.5f;
    public Animator Animator => animator;

    protected int _animIDSpeed;
    protected int _animIDGrounded;
    protected int _animIDJump;
    protected int _animIDFreeFall;
    protected int _animIDMotionSpeed;


    public void Start()
    {
        AssignAnimationIDs();
    }

    protected virtual void Update()
    {
        HandleAnimation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void HandleAnimation()
    {
        animator.SetBool(_animIDGrounded, movementController.Grounded);
        animator.SetFloat(_animIDSpeed, movementController.AnimationBlend);
        animator.SetFloat(_animIDMotionSpeed, movementController.InputMagnitude);
        animator.SetBool(_animIDJump, movementController.IsJumping);
        animator.SetBool(_animIDFreeFall, movementController.FreeFall);

        animator.SetFloat("VelocityX", movementController.InputAnimDirection.x);
        animator.SetFloat("VelocityY", movementController.InputAnimDirection.y);
    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting)
        {
            verticalAmount = 2;
        }

        //animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
        //animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = false, bool applyRootGravity = false, bool canRotate = false, bool canMove = false, bool showWeapon = true)
    {
        characterManager.SetApplyRootMotion(applyRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
        characterManager.SetIsPerformingAction(isPerformingAction);
        characterManager.SetCanRotate(canRotate);
        characterManager.SetCanMove(canMove);
        characterManager.SetApplyRootGravity(applyRootGravity);
        characterManager.SetShowWeapon(showWeapon);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation,int value, bool isPerformingAction, bool applyRootMotion = false, bool applyRootGravity = false, bool canRotate = false, bool canMove = false)
    {
        characterManager.SetApplyRootMotion(applyRootMotion);
        animator.SetInteger(targetAnimation, value);
        characterManager.SetIsPerformingAction(isPerformingAction);
        characterManager.SetCanRotate(canRotate);
        characterManager.SetCanMove(canMove);
        characterManager.SetApplyRootGravity(applyRootGravity);
    }

    public void CrossFade(string targetAnimation, float transitionDuration = 0.2f)
    {
        animator.CrossFade(targetAnimation, transitionDuration);
    }

    public void SetInt(string parameter, int index)
    {
        animator.SetInteger(parameter, index);
    }

    public void SetFloat(string parameter, float value)
    {
        animator.SetFloat(parameter, value);
    }

    public void SetBool(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
    }

    public void SetTrigger(string parameter)
    {
        animator.SetTrigger(parameter);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(characterManager.GetCharacterController.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterManager.GetCharacterController.center), FootstepAudioVolume);
        }
    }
}
