using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using Zenject;
public class Player : DamageableEntity
{

    //public Transform testSlope;
    public const float BASE_SPRINTSPEEDMOD = 1.5f;
    public const float BASE_CROUCHSPEEDMOD = 0.75f;

    public const float BASE_HEALTH = 100f;

    private float sprintSpeedMod;
    private float crouchSpeedMod;
    private PlayerMovement _playerMovement;
    private PlayerStats _stats;

    //Input keys can be refactored to scriptable objects 
    //this way player modified settings can be saved. 
    private KeyCode kc_jump = KeyCode.Space;
    private KeyCode kc_sprint = KeyCode.LeftShift;
    private KeyCode kc_crouch = KeyCode.LeftControl;

    // Start is called before the first frame update
    void Start()
    {
        this.Init();
    }

    //when we need to dynamically init the player
    void Init()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _stats = GetComponent<PlayerStats>();

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
        base._health = BASE_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        _playerMovement.Move(
            this.GetX(),
            this.GetZ(),
            _stats.MovementSpeed.GetCurrentValue(),
            _stats.JumpHeight.GetCurrentValue(),
            sprintSpeedMod,
            crouchSpeedMod,
            this.GetSprint(),
            this.GetCrouch(),
            this.GetJump()
            );
    }

    public float GetX()
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetZ()
    {
        return Input.GetAxis("Vertical");
    }

    public bool GetJump()
    {
        return Input.GetKey(kc_jump);
    }

    public bool GetSprint()
    {
        return Input.GetKey(kc_sprint);
    }

    public bool GetCrouch()
    {
        return Input.GetKey(kc_crouch);
    }
}
