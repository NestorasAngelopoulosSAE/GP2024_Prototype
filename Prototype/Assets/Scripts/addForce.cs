using UnityEngine;

public class addForce : MonoBehaviour
{
    [SerializeField] int speed = 10;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Colorable") || other.CompareTag("Player"))
        {
            if (!other.GetComponent<Green>() && other.gameObject.layer != LayerMask.GetMask("Held Object")) other.transform.position += transform.right * speed * Time.deltaTime;
        }
    }
}
