using UnityEngine;

public class addForce : MonoBehaviour
{
    [SerializeField] int speed = 10;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Colorable"))
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.right * speed , ForceMode.Impulse);
        }
    }
}
