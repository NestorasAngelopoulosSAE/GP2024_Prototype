/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Stores the velocity of the object and freezes it in place.
/// Once unfrozen, the object regains its velocity.
/// </summary>

using UnityEngine;

public class Green : MonoBehaviour
{
    Rigidbody rb;
    Vector3 storedVelocity;
    Vector3 storedAngularVelocity;

    void Start()
    {
        Debug.Log($"{gameObject.name} is Green!");
        rb = GetComponent<Rigidbody>();

        storedVelocity = rb.velocity;
        storedAngularVelocity = rb.angularVelocity;
        rb.isKinematic = true;
    }

    public void OnDisable()
    {
        Debug.Log($"{gameObject.name} is no longer Green!");

        rb.isKinematic = false;
        rb.velocity = storedVelocity;
        rb.angularVelocity = storedAngularVelocity;
    }

    void Update()
    {
        
    }
}
