
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MovementController : MonoBehaviour
{
    public Func<bool> CanMove;
    public Func<bool> CanRotate;
    public Func<bool> CanSprint;
    public Func<bool> CanJump;
    public event Action<bool> OnSprintChange;

    public Action OnDoubleJump;

    [SerializeField] private bool analogMovement;

    [SerializeField] private GameObject cinemachineCameraTarget;

    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float moveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float sprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float jumpHeight = 1.2f;

    [SerializeField] private float jumpCountMax = 2;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float fallTimeout = 0.15f;

    [SerializeField] private float groundedOffset = -0.14f;
    [SerializeField] private float groundedRadius = 0.28f;
    [SerializeField] private LayerMask groundLayers;

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
    private bool _isSprinting;
    private CharacterController _controller;
    private GameObject _mainCamera;
    private int jumpCount;

    public float AnimationBlend => _animationBlend;
    public float InputMagnitude => _inputMagnitude;
    public bool FreeFall => _freeFall;
    public bool IsJumping => _isJumping;
    public bool Grounded => grounded;
    public bool IsSprinting => sprint;
    public int GetJumpCount => jumpCount;

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
        bool canSprint = value && CanSprint.Invoke() && move.magnitude > 0.1f;

        if (sprint != canSprint)
        {
            sprint = canSprint;
            OnSprintChange?.Invoke(sprint);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main.gameObject;
        _controller = GetComponent<CharacterController>();

        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {

        if (CanMove != null && !CanMove())
            return;

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = sprint ? sprintSpeed : moveSpeed;

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

        float inputMagnitude = analogMovement ? move.magnitude : 1f;

        _speed = targetSpeed; 

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
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
                rotationSmoothTime);

            // rotate to face input direction relative to camera position

            if (CanRotate != null && !CanRotate())
                return;

            if (rotateOnMove)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
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
            _fallTimeoutDelta = fallTimeout;

            _isJumping = false;
            _freeFall = false;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (CanJump.Invoke() && jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                _isJumping = true;

                jumpCount = 1;
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            if (jump && jumpCount < jumpCountMax)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt((jumpHeight * 1.5f) * -2f * gravity);

                _isJumping = true;

                jumpCount++;

                OnDoubleJump?.Invoke();
            }

            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;

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
            _verticalVelocity += gravity * Time.deltaTime;
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
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }
}
