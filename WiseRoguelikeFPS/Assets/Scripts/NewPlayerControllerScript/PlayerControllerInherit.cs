using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerInherit : MonoBehaviour
{
    

    //Player Stats
    public float movementSpeed = 12f;
    public float jumpHeight = 10f;

    public float sprintSpeedMod = 1.5f;
    public float crouchSpeedMod = 0.75f;

    public float health = 100f;
    public float resistance = 0f;
    
    
    private List<Object> Items;


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
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        movement.Move(input.GetX(), input.GetZ(), movementSpeed, jumpHeight , sprintSpeedMod, crouchSpeedMod, input.GetSprint(), input.GetCrouch(), input.GetJump(), isGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
