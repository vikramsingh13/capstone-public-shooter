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
    private Vector3 movementVertical;

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
        movement = transform.right * x + transform.forward * z;

        Crouch(crouchInput);

        if (sprintInput && !isCrouching && !isSliding)
        {
            Debug.Log("Sprint");
            controller.Move(moveSpeed * sprintMod * Time.deltaTime * movement);
            isSprinting = true;
        }
        else if (isCrouching)
        {
            Debug.Log("Crouch");
            controller.Move(moveSpeed * crouchMod * Time.deltaTime * movement);
            isSprinting = false;
        }
        else
        {
            Debug.Log("Walk");
            controller.Move(moveSpeed * Time.deltaTime * movement);
            isSprinting = false;
        }

        if(jumpInput)
        {
            jump.Jump(isGrounded, gravity.GetGravity(), ref movementVertical);
        }

        gravity.Apply(isGrounded, ref movementVertical);

        controller.Move(movementVertical * Time.deltaTime);
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

}
