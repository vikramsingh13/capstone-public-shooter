using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    //Controllers
    GravityController gravity;
    JumpController jump;
    SlideController slide;

    //Player Variables
    private CharacterController controller;

    //Movement Variables
    private Vector3 movement;
    //private Vector3 movementVertical;

    public float raycastDistance = 1.5f;


    private Vector3 movementDirection;
    private float ySpeed;
    private float magnitude;

    //Boolean tracker variables
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isSliding = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        gravity = GetComponent<GravityController>();
        jump = GetComponent<JumpController>();
    }

    public void Move(float x = 0, 
        float z = 0, 
        float moveSpeed = 0, 
        float sprintMod = 0, 
        float crouchMod = 0,
        bool sprintInput = false,
        bool crouchInput = false,
        bool jumpInput = false,
        bool isGrounded = false)
    {

        movementDirection = transform.right * x + transform.forward * z;
        magnitude = Mathf.Clamp01(movementDirection.magnitude) * moveSpeed;
        movementDirection.Normalize();

        if (jumpInput)
        {
            jump.Jump(isGrounded, gravity.GetGravity(), ref ySpeed);
        }

        gravity.Apply(isGrounded, ref ySpeed);

        Crouch(crouchInput);

        if (sprintInput && !isCrouching && !isSliding)
        {
            Debug.Log("Sprint");
            magnitude *= sprintMod;
            isSprinting = true;
        }
        else if (isCrouching)
        {
            Debug.Log("Crouch");
            magnitude *= crouchMod;
            isSprinting = false;
        }
        else
        {
            Debug.Log("Walk");
            isSprinting = false;
        }

        movement = movementDirection * magnitude;
        movement = AdjustVelocityToSlope(movement);
        movement.y += ySpeed;

        controller.Move(movement * Time.deltaTime);
    }

    private void Crouch(bool crouchInput)
    {
        if (crouchInput)
        {
            if (!isCrouching && !isSprinting)
            {
                isCrouching = true;
                //Change player height
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                //Change player height
            }
        }
    }


    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if(adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

}
