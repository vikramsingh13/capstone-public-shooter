using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //Player Variables
    private CharacterController controller;

    public float gravity = -9.81f;

    //Movement Variables
    private Vector3 movement;

    public float raycastDistance = 1.5f;

    private Vector3 movementDirection;
    private float ySpeed;
    private float magnitude;

    //GroundCheck
    //Ground check empty game object near base of player
    [Tooltip("Empty game object near base of player")]
    public Transform groundCheck;

    //Radius of sphere created around groundCheck object
    [Tooltip("Radius of sphere created around groundCheck object")]
    public float groundDistance = 0.5f;

    //Layer mask to make sure the ground check only considers certain objects
    [Tooltip("Layer mask to make sure the ground check only considers certain objects")]
    public LayerMask groundMask;

    //Sliding variables
    private Vector3 slopeSlideVelocity;

    //Boolean flag variables
    private bool isCrouching = false;       //If crouch key is held down
    private bool isSprinting = false;       //If sprint key is held down
    private bool isSteepSliding = false;    //If player is sliding due to steep slope
    private bool isSliding = false;         //If player is sliding by command (Sprint + Crouch)
    private bool isGrounded = false;        //If player is touching the ground

    private void Start()
    {
        controller = GetComponent<CharacterController>();
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

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        movementDirection = transform.right * x + transform.forward * z;
        magnitude = Mathf.Clamp01(movementDirection.magnitude) * moveSpeed;
        movementDirection.Normalize();

        

        Gravity();

        SetSlopeVelocity();

        if(slopeSlideVelocity == Vector3.zero)
        {
            isSteepSliding = false;
        }
        else if(isGrounded && slopeSlideVelocity != Vector3.zero)
        {
            isSteepSliding = true;
        }

        Crouch(crouchInput);


        if(!isSteepSliding)
        {
            if (jumpInput)
            {
                Jump(jumpHeight);
            }

            if (sprintInput && !isCrouching)
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
        }

        movement = movementDirection * magnitude;
        movement = AdjustVelocityToSlope(movement);
        movement.y += ySpeed;

        if(isSteepSliding)
        {
            movement = slopeSlideVelocity;
            
            movement.y = ySpeed;
            
            movement.x += x * moveSpeed / 3;
            movement.z += z * moveSpeed / 3;

            Debug.DrawRay(transform.position, movement.normalized, Color.black);
            Debug.Log("Sloped Angle: " + Vector3.Angle(transform.forward, movement.normalized));
        }

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


    public void Jump(float jumpHeight)
    {
        if (isGrounded)
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


    //This method allows the player to move smoothly down slopes without "bumping"
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

    //Forces the player to slide down slopes that are too steep 
    private void SetSlopeVelocity()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 5))
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            Debug.Log("Slope Angle: " + angle);

            if (angle >= controller.slopeLimit)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, ySpeed, 0), hitInfo.normal);
                return;
            }
        }

        slopeSlideVelocity = Vector3.zero;

    }


    //Debugging--------------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

}
