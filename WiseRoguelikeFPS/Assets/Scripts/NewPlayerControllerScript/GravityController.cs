using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float gravity = -9.81f;

    public float GetGravity()
    {
        return gravity;
    }
    
    public void Apply(bool isGrounded, bool isSliding, ref float ySpeed)
    {
        if (isGrounded && !isSliding && ySpeed < 0)
        {
            ySpeed = -0.5f;
        }

        ySpeed += gravity * 2f * Time.deltaTime;
    }
}
