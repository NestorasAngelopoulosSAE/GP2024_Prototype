using UnityEngine;
using UnityEngine.Events;

public class Button3 : MonoBehaviour
{
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;      
    public float pressedY = 0.06f;
    public float startingY = 1.84f;
    public float speed = 2f;
    private Vector3 targetPosition;
    [SerializeField] private int ThingsOnMyButton;

    void Start()
    {      
        if (onButtonPressed == null)
            onButtonPressed = new UnityEvent();

        if (onButtonReleased == null)
            onButtonReleased = new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Colorable") || other.gameObject.CompareTag("Player"))
        {

            if (ThingsOnMyButton  == 0)
            {
                onButtonPressed.Invoke();
                Debug.Log("Button Pressed" + other.gameObject.name);             
            }
            ThingsOnMyButton++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
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