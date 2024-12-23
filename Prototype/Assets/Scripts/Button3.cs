/// <summary>
/// Thamnopoulos Thanos 2024
/// 
/// A button that creates a Unity event.
/// </summary>
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    // Unity event Vars...
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;  
    // Pressed, target and starting button positions...
    public float pressedY = 0.06f;
    public float startingY = 1.84f;
    private Vector3 targetPosition;
    // var that will allow us to calculate how many things are colliding with the button...
    [SerializeField] private int ThingsOnMyButton;
    // speed for button animation
    public float speed = 2f;

    void Start()
    {      
        // Creating the Unity events
        if (onButtonPressed == null)
            onButtonPressed = new UnityEvent();

        if (onButtonReleased == null)
            onButtonReleased = new UnityEvent();
    }

    // Button Collider TriggerEnter...
    private void OnTriggerEnter(Collider other)
    {
        // Button can interact with the player and Colorable objects...
        if (other.gameObject.CompareTag("Colorable") || other.gameObject.CompareTag("Player"))
        {
            //Button will only be invoked once when pressed until released...
            if (ThingsOnMyButton  == 0)
            {
                onButtonPressed.Invoke();
                Debug.Log("Button Pressed" + other.gameObject.name);             
            }
            ThingsOnMyButton++;
        }
    }

    //Button Collider TriggerExit...
    private void OnTriggerExit(Collider other)
    {
        // Button can interact with the player and Colorable objects...
        if (other.gameObject.CompareTag("Colorable") || other.gameObject.CompareTag("Player"))
        {
            ThingsOnMyButton--;

            if (ThingsOnMyButton == 0)
            {               
                onButtonReleased.Invoke();
                Debug.Log("Button Released" + other.gameObject.name);               
            }               
        }
    }     
  
    //Button Animation...
    void Update()
    {         
        if (ThingsOnMyButton > 0)
        {
            targetPosition = new Vector3(transform.position.x, pressedY, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);     
        }
        else
        {
            targetPosition = new Vector3(transform.position.x, startingY, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }
}