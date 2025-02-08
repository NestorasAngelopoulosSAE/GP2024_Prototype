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

    // Push object while in trigger.
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Colorable")) return;

        bool foundInList = false;
        foreach (Transform t in targets)
        {
            if (t == other.transform) foundInList = true;
        }
        if (!foundInList) targets.Add(other.transform);
    }

    private void Update()
    {
        // Slide texture on material.
        Vector3 offset = material.GetVector(_Offset);
        material.SetVector(_Offset, offset + new Vector3(materialSpeed * Time.deltaTime, 0, 0));

        // If object is no longer above the trigger, stop pushing it.
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].GetComponent<Green>() || targets[i].gameObject.layer == LayerMask.NameToLayer("Held Object"))
            {
                targets.RemoveAt(i);
                i--;
                continue;
            }

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
