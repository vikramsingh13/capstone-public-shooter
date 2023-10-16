using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController
{
    private KeyCode kc_jump = KeyCode.Space;
    private KeyCode kc_sprint = KeyCode.LeftShift;
    private KeyCode kc_crouch = KeyCode.LeftControl;


    public InputController()
    {

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
