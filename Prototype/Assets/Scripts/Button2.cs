using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Button2 : MonoBehaviour
{
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private bool isButtonPressed = false;
    private bool isAnimating = false;

    private Vector3 initialPosition;
    public float pressedY = 0.06f;
    public float startingY = 1.84f;
    public float pressSpeed = 5f;

    void Start()
    {
        initialPosition = new Vector3(transform.position.x, startingY, transform.position.z);

        if (onButtonPressed == null)
            onButtonPressed = new UnityEvent();

        if (onButtonReleased == null)
            onButtonReleased = new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Colorable") || other.gameObject.CompareTag("Player"))
        {
            if (!isButtonPressed)
            {
                isButtonPressed = true;
                onButtonPressed.Invoke();
                Debug.Log("Button Pressed");

                if (!isAnimating)
                    StartCoroutine(AnimateButtonPress(true));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Colorable") || other.gameObject.CompareTag("Player"))
        {
            if (isButtonPressed)
            {
                isButtonPressed = false;
                onButtonReleased.Invoke();
                Debug.Log("Button Released");

                if (!isAnimating)
                    StartCoroutine(AnimateButtonPress(false));
            }
        }
    }

    private IEnumerator AnimateButtonPress(bool press)
    {
        isAnimating = true;
        float animationTime = 0f;
        float targetYPosition = press ? pressedY : startingY;
        Vector3 targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);

        while (animationTime < 1f)
        {
            animationTime += Time.deltaTime * pressSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPosition, animationTime);
            yield return null;
        }

        transform.position = targetPosition;

        isAnimating = false;
    }

    void Update()
    {

    }
}