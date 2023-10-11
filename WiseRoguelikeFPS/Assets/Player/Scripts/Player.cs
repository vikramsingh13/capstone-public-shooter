using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const float BASE_SPRINTSPEEDMOD = 1.5f;
    public const float BASE_CROUCHSPEEDMOD = 0.75f;

    public const float BASE_HEALTH = 100f;

    //Player Current Stats
    private float sprintSpeedMod;
    private float crouchSpeedMod;

    private PlayerStats stats;

    //Contollers
    private InputController input;
    private MovementController movement;

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

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        input = new InputController();
        stats = GetComponent<PlayerStats>();

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


        movement.Move(
            input.GetX(), 
            input.GetZ(), 
            stats.MovementSpeed.GetCurrentValue(), 
            stats.JumpHeight.GetCurrentValue(), 
            sprintSpeedMod, 
            crouchSpeedMod, 
            input.GetSprint(), 
            input.GetCrouch(), 
            input.GetJump(), 
            isGrounded
            );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

}
