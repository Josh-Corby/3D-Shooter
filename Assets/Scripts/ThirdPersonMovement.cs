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
    public int midAirJumpCount;
    public int midAirJumpsLeft;

    private float coyoteTime = 0.1f;
    public float coyoteTimeCounter;

    public Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private Vector2 movementVector;
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
        if (IsGrounded())
        {
            ResetMidairJumps();
            coyoteTimeCounter = coyoteTime;

            if(velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
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
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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
        if (coyoteTimeCounter > 0f)
        {
                JumpForce();         
        }
        else
        {
            if (midAirJumpsLeft > 0)
            {
                midAirJumpsLeft -= 1;
                JumpForce();
            }
        }
    }

    private void JumpForce()
    {
        coyoteTimeCounter = 0f;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);   
    }



    private void ResetMidairJumps()
    {
        midAirJumpsLeft = midAirJumpCount;    
    }
    public void RecieveInput(Vector2 input)
    {
        movementVector = input;
    }
}