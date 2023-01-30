using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : GameBehaviour<InputManager>
{
    public static event Action Jump = null;
    public static event Action Fire = null;
    public static event Action StopFiring = null;
    public static event Action ToggleSprint = null;
    ThirdPersonMovement movement;
    PlayerInput controls;
    PlayerInput.InputActions inputActions;
    public Vector2 movementVector, cameraInput;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        movement = GetComponent<ThirdPersonMovement>();
        if (controls == null)
        {
            controls = new PlayerInput();

            controls.Input.Movement.performed += i => Move(i.ReadValue<Vector2>());
            controls.Input.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

            controls.Input.Jump.performed += i => Jump?.Invoke();

            controls.Input.Sprint.performed += i => ToggleSprint?.Invoke();
            controls.Input.Sprint.canceled += i => ToggleSprint?.Invoke();

            controls.Input.Fire.performed += i => Fire?.Invoke();
            controls.Input.Fire.canceled += i => StopFiring?.Invoke();

            EnableControls();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Move(Vector2 direction)
    {
        movement.RecieveInput(direction);
    }

    private void OnDisable()
    {
        DisableControls();
    }

    public void DisableControls()
    {
        controls.Disable();
    }

    public void EnableControls()
    {
        controls.Enable();
    }
}
