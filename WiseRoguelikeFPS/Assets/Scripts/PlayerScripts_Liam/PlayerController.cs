using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Character movement parameters
    public CharacterController myCharacterController;
    public float speed = 12f;
    public Vector3 velocity;

    private float crouchSpeedModifier = 1;
    private float sprintSpeedModifier = 2;

    private bool isRunning = false;

    //jumping
    public float gravityModifier = 15f;
    public float jumpHeight = 5f;
    private bool readyToJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    //Crouching
    public Transform myBody;
    private float initialControllerHeight;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 bodyScale;
    private bool isCrouching;

    //Sliding
    private float slideSpeed;
    private float currentSlideTimer, maxSlideTime = 2f;
    private bool startSliderTimer;
    public Vector3 slideDirection;

    //For camera movement
    public Transform myCameraHead;
    public float sensitivity = 400f;
    private float cameraVerticalRotation;

    // Start is called before the first frame update
    void Start()
    {
        bodyScale = myBody.localScale;
        initialControllerHeight = myCharacterController.height;
        slideSpeed = speed * sprintSpeedModifier;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        CameraMovement();
        Jump();
        Gravity();
        Crouch();
        SlideCounter();
    }

    private void Jump()
    {
        readyToJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;

        if(Input.GetButtonDown("Jump") && readyToJump)
        {
            velocity.y = Mathf.Sqrt((jumpHeight / -1000f) * Physics.gravity.y);
            myCharacterController.Move(velocity);
        }
    }

    private void Gravity()
    {
        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        if (myCharacterController.isGrounded)
            velocity.y = 0f; //Physics.gravity.y * Time.deltaTime;


        myCharacterController.Move(velocity);
    }

    private void PlayerMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = x * transform.right + z *transform.forward;

        if(Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            slideSpeed = sprintSpeedModifier * speed * Time.deltaTime;
            movement *= slideSpeed;
            isRunning = true;
        }
        else
        {
            movement *= crouchSpeedModifier * speed * Time.deltaTime;
            isRunning = false;
        }

        slideDirection = new Vector3(x, 0, z);
        myCharacterController.Move(movement);
    }

    private void Crouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouching();
        }
        if (isCrouching && Input.GetKeyUp(KeyCode.LeftControl) || currentSlideTimer > maxSlideTime)
        {
            StopCrouching();
        }
    }

    private void StartCrouching()
    {
        myBody.localScale = crouchScale;
        myCameraHead.position -= new Vector3(0, 1.3f, 0);
        myCharacterController.height /= 2;
        crouchSpeedModifier = 0.4f;
        isCrouching = true;
        
        //Slide
        if (isRunning)
        {
            Vector3 slideDirectionFinal = slideDirection.magnitude > 0 ? slideDirection.normalized : transform.forward;
            slideDirectionFinal = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * slideDirectionFinal;
            velocity = slideDirectionFinal.normalized * slideSpeed;
            //velocity = Vector3.ProjectOnPlane(slideDirectionFinal.normalized, Vector3.up).normalized * slideSpeed * Time.deltaTime;
            startSliderTimer = true;
        }
    }

    private void StopCrouching()
    {
        currentSlideTimer = 0f;
        velocity = new Vector3(0, 0, 0);
        startSliderTimer = false;

        myBody.localScale = bodyScale;
        myCameraHead.position += new Vector3(0, 1.3f, 0);
        myCharacterController.height = initialControllerHeight;
        crouchSpeedModifier = 1;
        isCrouching = false; 
    }

    private void SlideCounter()
    {
        if(startSliderTimer)
        {
            currentSlideTimer += Time.deltaTime;
        }
    }


    private void CameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90, 90);

        
        transform.Rotate(Vector3.up * mouseX);
        myCameraHead.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }
}
