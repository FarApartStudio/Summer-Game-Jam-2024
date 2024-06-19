using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Action OnPauseBtnPressed;

    [SerializeField] private Vector2 movementInput;
    private PlayerControls playerControls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerControls = new PlayerControls();
            playerControls.Player.Pause.performed += Pause_performed;
        }
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseBtnPressed?.Invoke();
    }

    public void ToggleCursor(bool value)
    {
        Cursor.lockState = value ?  CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }


    public Vector2 GetMovementInput()
    {
       return  playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetCameraInput()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool GetJumpInput()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool GetDodgeInput()
    {
        return playerControls.Player.Dodge.triggered;
    }

    public bool GetSprintInput()
    {
        return playerControls.Player.Sprint.IsPressed();
    }

    public bool GetAimInput()
    {
        return playerControls.Player.Aim.IsPressed();
    }

    public bool GetInteractInput()
    {
        return playerControls.Player.Interact.triggered;
    }

    public bool GetReloadInput()
    {
        return playerControls.Player.Reload.triggered;
    }

    public InputAction GetAttackInputAction()
    {
        return playerControls.Player.Attack;
    }

    public InputAction GetAimInputAction()
    {
        return playerControls.Player.Aim;
    }

    public InputAction GetInteractInputAction()
    {
        return playerControls.Player.Interact;
    }

    public bool GetSwapWeaponInput()
    {
        return playerControls.Player.SwapWeapon.triggered;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnDestroy()
    {
        playerControls.Player.Pause.performed -= Pause_performed;
        playerControls.Dispose();
    }
}
