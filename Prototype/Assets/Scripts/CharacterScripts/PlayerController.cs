using UnityEngine;
/// <summary>
/// Thamnopoulos Thanos 2024
/// 
/// Basic Player Controller.
/// </summary>

public class PlayerController : MonoBehaviour
{   
    Transform mainCamera;
    CharacterController characterController;

    public float speed = 10;
    public float rotationSpeed = 2;

    // Movement Inputs...
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
    public float jumpForce = 2.5f;
    bool isGrounded = false;

    //Direction
    Vector3 direction;
    Vector3 jumpDirection = Vector3.zero;

    UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            rotationSpeed = PlayerPrefs.GetFloat("Sensitivity");
        }

        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

        characterController.center = Vector3.up;

       // mainCamera.position += Vector3.up * 1.8f;

        uiManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiManager.isPaused) return;
        
        // Keep player upright even if a moving platfrom tries to rotate them.
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0f, eulerAngles.y, 0f));

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
            transform.SetParent(null, true); // Make sure the player is off any moving platform.
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


