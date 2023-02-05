using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : GameBehaviour<InputManager>
{
    public static event Action OnJump = null;
    public static event Action OnFire = null;
    public static event Action OnStopFiring = null;
    public static event Action OnReload = null;
    public static event Action OnToggleSprint = null;
    public static event Action<Vector2> OnMove = null;
    public static event Action<Vector2> OnLook = null;
    public static event Action<float> OnScroll = null;
    public static event Action OnInteract = null;


    PlayerInput controls;
    public Vector2 scrollInput;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (controls == null)
        {
            controls = new PlayerInput();

            controls.Input.Movement.performed += i => OnMove(i.ReadValue<Vector2>());
            controls.Input.Look.performed += i => OnLook(i.ReadValue<Vector2>());
            controls.Input.Jump.performed += i => OnJump?.Invoke();
            controls.Input.Sprint.performed += i => OnToggleSprint?.Invoke();
            controls.Input.Sprint.canceled += i => OnToggleSprint?.Invoke();

            controls.Input.Fire.performed += i => OnFire?.Invoke();
            controls.Input.Fire.canceled += i => OnStopFiring?.Invoke();
            controls.Input.Reload.performed += i => OnReload?.Invoke();
            controls.Input.ChangeWeapon.performed += i => OnScroll(i.ReadValue<float>());
            controls.Input.Interact.performed += i => OnInteract?.Invoke();

            EnableControls();
            Cursor.lockState = CursorLockMode.Locked;
        }
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
