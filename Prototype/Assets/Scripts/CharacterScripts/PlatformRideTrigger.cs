/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes it possible for objects to ride on top of blue platforms.
/// </summary>
using UnityEngine;

public class PlatformRideTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Colorable" &! other.gameObject.GetComponent<Green>())
        {
            other.gameObject.transform.SetParent(transform.parent, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.transform.SetParent(null, true);
    }
}
