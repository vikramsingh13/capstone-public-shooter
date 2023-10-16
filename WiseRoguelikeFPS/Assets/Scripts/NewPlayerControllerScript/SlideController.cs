using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : MonoBehaviour
{

    private CharacterController controller;

    public float slideMaxTime = 3f;
    private float slideTimer = 0f;

    private Vector3 slideVelocity;
    private Vector3 slideDirection;

    public Transform cameraHead;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    /*
    private void Slide(ref bool isSliding)
    {
        if (isSprinting && Input.GetKeyDown(kc_crouch) && !isCrouching && !isSliding)
        {
            slideDirection = controller.velocity.normalized;
            slideTimer = 0f;
            isSliding = true;
        }

        if (isSliding)
        {
            if (Input.GetKeyUp(kc_crouch))
            {
                isSliding = false;
            }

            slideTimer += Time.deltaTime;

            if (slideTimer < slideMaxTime)
            {
                controller.Move(slideDirection * moveSpeed * (2f - (2f * (slideTimer / slideMaxTime))) * Time.deltaTime);
            }
            else
            {
                isSliding = false;
            }
        }
    }*/
}
