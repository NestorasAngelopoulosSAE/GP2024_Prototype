/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes it possible for objects to ride on top of blue platforms.
/// Manages a stack of objects, all moving together on top of a blue platform.
/// </summary>
using UnityEngine;

public class RideTrigger : MonoBehaviour
{
    private void Start()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        transform.localScale = Vector3.one * 0.95f; // Make trigger slightly smaller so that it doesn't trigger from the sides.
    }

    private void Update()
    {
        // Make sure the ride trigger is always on top of the object, regardless of orientation.
        transform.localPosition = transform.InverseTransformDirection(Vector3.up) * 0.1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (transform.parent.gameObject.layer != LayerMask.NameToLayer("Held Object") && (other.tag == "Player" || other.tag == "Colorable" &! other.GetComponent<Green>() &! other.GetComponent<Blue>()) &! other.GetComponent<RideTrigger>())
        {
            other.transform.SetParent(transform.parent, true);
        }
    }

    // Make sure that if an object that's riding on the platform changes properties, it will stop following it.
    private void OnTriggerStay(Collider other)
    {
        if (transform.parent.gameObject.layer == LayerMask.NameToLayer("Held Object") || other.GetComponent<Green>() || other.GetComponent<Blue>())
        {
            OnTriggerExit(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disconnect from platform.
        if (other.transform.GetComponent<RideTrigger>() || other.tag == "Button") return;
        other.transform.SetParent(null, true);
    }
}
