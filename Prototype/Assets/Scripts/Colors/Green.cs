/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Stores the velocity of the object and freezes it in place.
/// Once unfrozen, the object regains its velocity.
/// </summary>

using UnityEngine;

public class Green : MonoBehaviour
{
    Rigidbody rigidBody;
    Vector3 storedVelocity;
    Vector3 storedAngularVelocity;

    void Start()
    {
        //Debug.Log($"{gameObject.name} is Green!");
        rigidBody = GetComponent<Rigidbody>();

        // Lift object so it doesn't overlap with a moving platform.
        if (transform.parent != null && transform.parent.gameObject.GetComponent<Blue>()) transform.position += Vector3.up * 0.01f;

        storedVelocity = rigidBody.velocity;
        storedAngularVelocity = rigidBody.angularVelocity;
        rigidBody.isKinematic = true;
    }

    public void OnDisable()
    {
        //Debug.Log($"{gameObject.name} is no longer Green!");

        rigidBody.isKinematic = false;
        rigidBody.velocity = storedVelocity;
        rigidBody.angularVelocity = storedAngularVelocity;
    }
}
