/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Manage objects entering a river-like trigger object that pushes the player and colorable objects aside.
/// Keep applying the force even when the object leaves the trigger (the player jumps), as longs as it is still above the trigger.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] float forceSpeed = 50;
    List<Transform> targets = new List<Transform>();

    Material material;
    int _Offset = Shader.PropertyToID("_Texture_Offset");
    [SerializeField] float materialSpeed = 2;

    private void Start() => material = GetComponent<MeshRenderer>().material;

    #region Colorable
    // Push object while on conveyor.
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Colorable") && !collision.gameObject.GetComponent<Green>() && collision.gameObject.layer != LayerMask.NameToLayer("Held Object"))
        {
            collision.transform.position += transform.right * forceSpeed * Time.deltaTime;
        }
    }

    // Add to list for raycasting when exiting conveyor.
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Colorable") && !collision.gameObject.GetComponent<Green>() && collision.gameObject.layer != LayerMask.NameToLayer("Held Object"))
        {
            targets.Add(collision.transform);
        }
    }
    #endregion

    #region Player
    // Push player while in trigger.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.position += transform.right * forceSpeed * Time.deltaTime;
    }

    // Add to list for raycasting when exiting trigger.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) targets.Add(other.transform);
    }
    #endregion

    private void Update()
    {
        // Slide texture on material.
        Vector3 offset = material.GetVector(_Offset);
        material.SetVector(_Offset, offset + new Vector3(materialSpeed * Time.deltaTime, 0, 0));

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
            t.position += transform.right * forceSpeed * Time.deltaTime;
        }
    }
}
