using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController
{
    public void Jump(bool isGrounded, float jumpHeight , float gravity, ref float ySpeed)
    {
        if (isGrounded)
        {
            ySpeed = Mathf.Sqrt(jumpHeight* (-2f * gravity));
        }
    }
}
