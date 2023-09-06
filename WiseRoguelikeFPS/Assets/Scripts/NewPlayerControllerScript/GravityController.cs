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
    
    public void Apply(bool isGrounded, ref Vector3 verticalVelocityVector)
    {
        if (isGrounded && verticalVelocityVector.y < 0)
        {
            verticalVelocityVector.y = -0.5f;
        }

        verticalVelocityVector.y += gravity * 2f * Time.deltaTime;
    }
}
