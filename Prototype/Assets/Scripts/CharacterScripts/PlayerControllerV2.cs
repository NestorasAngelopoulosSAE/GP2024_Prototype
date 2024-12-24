using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Thamnopoulos Thanos 2024
/// 
/// Basic Player Controller.
/// </summary>

public class PlayerController : MonoBehaviour
{
    // I DO DECLARE...
    Transform mainCamera;
    CharacterController characterController;

    public float speed = 20;
    public float rotationSpeed = 2;

    // Mmovement Inputs...
    float inpHor;
    float inpVer;

    //Look Inputs...
    float mouseHor;
    float mouseVert;
    float mouseInvertX = 1;
    float mouseInvertY = -1;

    //Camera Rotation...
    float cameraRotationX;

    //Gravity...
    float gravity = 15;
    public float airControl = 1.5f;
    public float jumpForce = 3;
    bool isGrounded = false;

    //Direction
    Vector3 direction;
    Vector3 jumpDirection = Vector3.zero;

    UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

        characterController.center = Vector3.up;

       // mainCamera.position += Vector3.up * 1.8f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiManager.isPaused) return;

        isGrounded = characterController.isGrounded;

        //inputs
        inpHor = Input.GetAxis("Horizontal");
        inpVer = Input.GetAxis("Vertical");
        mouseHor = Input.GetAxis("Mouse X");
        mouseVert = Input.GetAxis("Mouse Y");

        //rotate player first
        transform.Rotate(0, mouseHor * rotationSpeed * mouseInvertX, 0);

        direction = (transform.right * inpHor) + (transform.forward * inpVer);

        Vector3 moveDirection = direction.normalized;

        if (isGrounded == true)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpDirection.y = jumpForce;
            }
            else
            {
                jumpDirection.y = -1;
            }
        }
        else
        {
            moveDirection *= airControl;
        }

        jumpDirection.y -= gravity * Time.deltaTime;


        // move
        characterController.Move(moveDirection * speed * Time.deltaTime);
        //apply gravity
        characterController.Move(jumpDirection * gravity * Time.deltaTime);
        //finalising rotation aspects (clamp on -85,85)...
        cameraRotationX += mouseVert * rotationSpeed * mouseInvertY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -85, 85);
        mainCamera.localEulerAngles = new Vector3(cameraRotationX, 0, 0);

    }
}


