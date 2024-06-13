
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public Func<bool> CanMove;
    public Func<bool> CanRotate;
    public Func<bool> CanSprint;
    public event Action<bool> OnSprintChange;

    [SerializeField] private bool analogMovement;

    [SerializeField] private GameObject cinemachineCameraTarget;

    [SerializeField] private MovementDataSO movementDataSO;

    private bool grounded = true;
    private float _speed;

    private float _animationBlend;
    private float _inputMagnitude;
    private bool _freeFall;
    private bool _isJumping;

    private Vector2 _animationInputMagnitude;
    private Vector3 targetDirection;

    private bool rotateOnMove = true;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    public Vector2 InputAnimDirection => _animationInputMagnitude;
    public Vector2 TargetDirection => targetDirection;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;
    private GameObject _mainCamera;

    public float AnimationBlend => _animationBlend;
    public float InputMagnitude => _inputMagnitude;
    public bool FreeFall => _freeFall;
    public bool IsJumping => _isJumping;
    public bool Grounded => grounded;

    public bool IsSprinting => sprint;

    public GameObject CinemachineCameraTarget => cinemachineCameraTarget;

    [Header("Controls")]
    public Vector2 move;
    public bool jump;
    public bool sprint;

    public void OnMove(Vector2 value)
    {
        move = value;
    }

    public void OnJump(bool value)
    {
        jump = value;
    }

    public void OnSprint(bool value)
    {
        sprint = value;
    }

    public void OnSprint()
    {
        if (CanSprint != null && !CanSprint())
            return;

        sprint = !sprint;
        OnSprintChange?.Invoke(sprint);
    }


    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = movementDataSO.JumpTimeout;
        _fallTimeoutDelta = movementDataSO.FallTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - movementDataSO.GroundedOffset,
            transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, movementDataSO.GroundedRadius, movementDataSO.GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        if (CanMove != null && !CanMove())
            return;

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = sprint ? movementDataSO.SprintSpeed : movementDataSO.MoveSpeed;

        _animationInputMagnitude =  Vector2.Lerp(_animationInputMagnitude, sprint ? new Vector2(0, 2) : move, Time.deltaTime * 5f);

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (move == Vector2.zero)
        {
            targetSpeed = 0.0f;
            StopSprint();
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = analogMovement ? move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * movementDataSO.SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * movementDataSO.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                movementDataSO.RotationSmoothTime);

            // rotate to face input direction relative to camera position

            if (CanRotate != null && !CanRotate())
                return;
            if (rotateOnMove)
            {
                if(sprint)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                else
                {
                    transform.rotation = Quaternion.Euler(0.0f, _mainCamera.transform.eulerAngles.y, 0.0f);
                }
            }
        }


        targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _inputMagnitude = inputMagnitude;
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = movementDataSO.FallTimeout;

            _isJumping = false;
            _freeFall = false;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(movementDataSO.JumpHeight * -2f * movementDataSO.Gravity);

                _isJumping = true;
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = movementDataSO.JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _freeFall = true;
            }

            // if we are not grounded, do not jump
            jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += movementDataSO.Gravity * Time.deltaTime;
        }
    }

    public void SetRotateOnMove(bool rotate)
    {
        rotateOnMove = rotate;
    }

    public void StopMovement()
    {
        move = Vector2.zero;
        sprint = false;
        jump = false;
        StopSprint();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Checks if the player collided with a object in front of them
        float dotProduct = Vector3.Dot(hit.normal, transform.up);

        if (dotProduct > 0.1)
        {
            return;
        }

        StopSprint();
    }

    public void StopSprint()
    {
        if (sprint)
        {
            sprint = false;
            OnSprintChange?.Invoke(sprint);
        }

    }

    private void OnDrawGizmos()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - movementDataSO.GroundedOffset, transform.position.z),
            movementDataSO.GroundedRadius);
    }
}
