using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController
{
    public float jumpHeight = 10f;

    public void Jump(bool isGrounded, float gravity, ref float ySpeed)
    {
        if (isGrounded)
        {
            ySpeed = Mathf.Sqrt(jumpHeight* (-2f * gravity));
        }
    }
}
