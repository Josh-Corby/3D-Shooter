using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : GameBehaviour<InputManager>
{
    public static event Action Jump = null;
    public static event Action Fire = null;
    public static event Action StopFiring = null;
    public static event Action Reload = null;
    public static event Action ToggleSprint = null;
    public static event Action<Vector2> Move = null;
    public static event Action<Vector2> Look = null;
    public static event Action<float> Scroll = null;


    PlayerInput controls;
    public Vector2 scrollInput;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (controls == null)
        {
            controls = new PlayerInput();

            controls.Input.Movement.performed += i => Move(i.ReadValue<Vector2>());
            controls.Input.Look.performed += i => Look(i.ReadValue<Vector2>());
            controls.Input.Jump.performed += i => Jump?.Invoke();
            controls.Input.Sprint.performed += i => ToggleSprint?.Invoke();
            controls.Input.Sprint.canceled += i => ToggleSprint?.Invoke();

            controls.Input.Fire.performed += i => Fire?.Invoke();
            controls.Input.Fire.canceled += i => StopFiring?.Invoke();
            controls.Input.Reload.performed += i => Reload?.Invoke();
            controls.Input.ChangeWeapon.performed += i => Scroll(i.ReadValue<float>());;

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
