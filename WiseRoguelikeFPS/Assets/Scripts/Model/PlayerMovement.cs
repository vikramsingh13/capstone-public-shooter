                                                                                                                                                                      using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    //Player Variables
    [SerializeField]
    private CharacterController _characterController;


    //Movement Variables
    [SerializeField] private float gravity = -9.81f;

    private float ySpeed;               //Track Up/Down speed

    private float currentMoveSpeed = 0f;

    private Vector3 movementDirection;  //Stores the normal of the vector created by X and Z (ie the direction)
    private float speedMagnitude;       //Used to store the magnitude of movementDirection * moveSpeed to find the correct movement speed at any angle
    private Vector3 movement;           //Vector to store the movement of the character, (direction * magnitude)


    //GroundCheck
    public Transform groundCheck;            //Ground check empty game object near base of player
    public float groundDistance = 0.5f;      //Radius of sphere created around groundCheck object
    public LayerMask groundMask;             //Layer mask to make sure the ground check only considers certain objects


    //Sliding variables
    /*
    [SerializeField] private float slideMaxTime = 3f;
    private float slideTimer = 0f;

    private bool startSlideTimer = false;

    private Vector3 slideVelocity;
    private Vector3 slideDirection;*/

    //Slope handling variables
    [SerializeField] private float slopeRaycastDistance = 1.5f;
    private Vector3 slopeSlideVelocity;

    //Boolean flag variables
    private bool isCrouching = false;       //If player is holding crouch key
    private bool isSprinting = false;
    private bool isSteepSliding = false;    //If player is sliding due to steep slope
    //private bool isSliding = false;         //If player is sliding by command (Sprint + Crouch)
    private bool isGrounded = false;        //If player is touching the ground


    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
            Debug.Log($"char controller: {_characterController}");
        }

    }

    public void Move(float x = 0, 
        float z = 0, 
        float moveSpeed = 0, 
        float jumpHeight = 0,
        float sprintMod = 0, 
        float crouchMod = 0,
        bool sprintInput = false,
        bool crouchInput = false,
        bool jumpInput = false)
    {
        Gravity();

        HandleSprintCrouchSlideInput(sprintInput, crouchInput, moveSpeed);

        CalculateBaseMovement(x, z, moveSpeed, sprintMod, crouchMod);

        SetSlopeVelocity();

        if(!isSteepSliding)
        {
            Jump(jumpInput, jumpHeight);
        }
        

        CalculateFinalMovement();

        if (isSteepSliding)
        {
            movement += slopeSlideVelocity;
        }

        //Debug.DrawLine(transform.position, transform.position + movement, Color.green, 0.5f, false);

        currentMoveSpeed = movement.magnitude;

        _characterController.Move(movement * Time.deltaTime);


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void AerialMovement(
        float x = 0,
        float z = 0,
        float moveSpeed = 0
        )
    {
        movement.z += z * moveSpeed;
        movement.x += x * moveSpeed;
    }

    private void CalculateBaseMovement(float x, float z, float moveSpeed, float sprintMod, float crouchMod)
    {
        movementDirection = transform.right * x + transform.forward * z;                //Find horizontal movement based on player input
        speedMagnitude = Mathf.Clamp01(movementDirection.magnitude) * moveSpeed * (isSprinting ? sprintMod : isCrouching ? crouchMod : 1); ;       //Find the proper speed of the movement
        movementDirection.Normalize();                                                  //Normalize to get the direction of the movement
    }

    private void CalculateFinalMovement()
    {
        movement = movementDirection * speedMagnitude;      //Find the movement vector (direction * speed)
        movement = AdjustVelocityToSlope(movement);         //Adjust the direction of the vector to walk smoothly down slopes (that arent steep)
        movement.y += ySpeed;                               //Apply ySpeed, (jumping and gravity)
    }

    private void HandleSprintCrouchSlideInput(bool sprintInput , bool crouchInput, float moveSpeed)
    {
        if (!isSteepSliding)
        {
            if (sprintInput || isSprinting)
            {
                isSprinting = true;

                if (!sprintInput && isSprinting)
                {
                    isSprinting = false;
                }
            }
            if (crouchInput || isCrouching)
            {
                isCrouching = true;

                if(!crouchInput && isCrouching)
                {
                    isCrouching = false;
                }
            }
        }
    }

    public void Jump(bool jumpInput, float jumpHeight)
    {
        if (isGrounded && jumpInput)
        {
            ySpeed = Mathf.Sqrt(jumpHeight * (-2f * gravity));
        }
    }

    public void Gravity()
    {
        if (isGrounded && !isSteepSliding && ySpeed < 0)
        {
            ySpeed = -0.5f;
        }
        else if (isSteepSliding && ySpeed < -30)
        {
            ySpeed = -30f;
        }

        ySpeed += gravity * 4f * Time.deltaTime;


    }

    /*
    private void Slide()
    {
        Debug.Log("SLIDING");
        if (startSlideTimer)
        {
            slideDirection = transform.forward;
            slideTimer = 0f;
            startSlideTimer = false;
        }
        if (!isCrouching)
        {
            isSliding = false;
        }

        slideTimer += Time.deltaTime;

        if (slideTimer < slideMaxTime)
        {
            controller.Move(slideDirection * currentMoveSpeed * Time.deltaTime);
            currentMoveSpeed *= 2f - (2f * (slideTimer / slideMaxTime));
        }
        else
        {
            isSliding = false;
        }
    }*/


    //Allows the player to move smoothly down slopes without "bumping"
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, slopeRaycastDistance))
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

    //Forces the player to slide down slopes that are too steep 
    private void SetSlopeVelocity()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 5))
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            //Debug.Log("Slope Angle: " + angle);

            if (angle >= _characterController.slopeLimit)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, ySpeed, 0), hitInfo.normal);
                isSteepSliding = true;

                return;
            }
        }
        isSteepSliding = false;
        slopeSlideVelocity = Vector3.zero;

    }


    //Debugging--------------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

}
