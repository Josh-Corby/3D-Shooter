using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    private bool isSprinting;
    public float currentMoveSpeed;
    public float walkSpeed = 6f;
    public float sprintSpeed = 10;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;
    Vector3 velocity;
    private Vector2 movementVector;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private void Start()
    {
        currentMoveSpeed = walkSpeed;
        InputManager.Jump += Jump;
        InputManager.ToggleSprint += ToggleSprint;
    }
    private void Update()
    {
        LookFoward();
        PlayerMove();
    }

    private void PlayerMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (movementVector.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementVector.x, movementVector.y) * Mathf.Rad2Deg + cam.eulerAngles.y;           
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentMoveSpeed * Time.deltaTime);

        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ToggleSprint()
    {
        isSprinting = !isSprinting;
        currentMoveSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }
    
    private void LookFoward()
    {
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

    }
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    public void RecieveInput(Vector2 input)
    {
        movementVector = input;
    }
}