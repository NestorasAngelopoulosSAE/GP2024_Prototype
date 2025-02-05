/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Manage objects entering a river-like trigger object that pushes the player and colorable objects aside.
/// Keep applying the force even when the object leaves the trigger (the player jumps), as longs as it is still above the trigger.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    [SerializeField] int speed = 10;
    List<Transform> targets = new List<Transform>();

    // Push object while in trigger.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Colorable") || other.CompareTag("Player"))
        {
            if (!other.GetComponent<Green>() && other.gameObject.layer != LayerMask.NameToLayer("Held Object"))
            {
                other.transform.position += transform.right * speed * Time.deltaTime;
            }
        }
    }

    // Add to list for raycasting when exiting trigger.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Colorable") || other.CompareTag("Player"))
        {
            if (!other.GetComponent<Green>() && other.gameObject.layer != LayerMask.NameToLayer("Held Object"))
            {
                targets.Add(other.transform);
            }
        }
    }

    private void Update()
    {
        // If object is no longer above the trigger, stop pushing it.
        for (int i = 0; i < targets.Count; i++)
        {
            if (Physics.Raycast(targets[i].position, Vector3.down, out RaycastHit hit, 2f))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    targets.RemoveAt(i);
                    i--;
                }
            }
        }

        // Apply push to objects above trigger.
        foreach (Transform t in targets)
        {
            t.position += transform.right * speed * Time.deltaTime;
        }
    }
}
