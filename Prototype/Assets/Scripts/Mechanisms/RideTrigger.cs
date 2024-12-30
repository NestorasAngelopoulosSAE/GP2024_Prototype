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
        if (other.tag == "Player") // Player
        {
            other.transform.SetParent(transform.root, true);
        }
        else if (other.tag == "Colorable" &! other.GetComponent<Green>()) // Movable
        {
            other.transform.SetParent(transform.root, true);

            // Don't add a Ride Trigger if there is already one there.
            for (int i = 0; i < other.transform.childCount; i++)
            {
                if (other.transform.GetChild(i).GetComponent<RideTrigger>()) return;
            }

            // Add another ride trigger to any object on the platform so that a stack can be created.
            Blue.CreateRideTriggerObject(other, out RideTrigger RideTrigger);
        }
    }

    // Make sure that if an object that's riding on the platform turns green, it will stop following it.
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Green>())
        {
            OnTriggerExit(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disconnect from platform.
        other.transform.SetParent(null, true);

        // Find the next trigger on the stack and repeat.
        for (int i = 0; i < other.transform.childCount; i++)
        {
            RideTrigger trigger = other.transform.GetChild(i).GetComponent<RideTrigger>();
            if (trigger)
            {
                trigger.Remove();
            }
        }
    }

    // This couldn't be implemented in OnDestroy() because marking the object for destruction prevents us from targetting its parent.
    public void Remove()
    {
        if (transform.parent != null)
        {
            Transform parent = transform.parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                if (!parent.GetChild(i).GetComponent<RideTrigger>()) // Disconnect any siblings.
                {
                    OnTriggerExit(parent.GetChild(i).GetComponent<Collider>());
                }
            }
        }
        Destroy(gameObject);
    }
}
