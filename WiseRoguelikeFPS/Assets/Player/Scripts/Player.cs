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

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MovementController>();
        input = GetComponent<InputController>();
        stats = GetComponent<PlayerStats>();

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
    }

    // Update is called once per frame
    void Update()
    {
        movement.Move(
            input.GetX(), 
            input.GetZ(), 
            stats.MovementSpeed.GetCurrentValue(), 
            stats.JumpHeight.GetCurrentValue(), 
            sprintSpeedMod, 
            crouchSpeedMod, 
            input.GetSprint(), 
            input.GetCrouch(), 
            input.GetJump()
            );
    }

}
