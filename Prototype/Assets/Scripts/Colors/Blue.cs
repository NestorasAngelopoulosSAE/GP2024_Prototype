using UnityEngine;

public class Blue : MonoBehaviour
{
    [SerializeField] public float speed = 8f;
    Vector3 moveDirection;
    Rigidbody RB;

    void Start()
    {
        //Debug.Log($"{gameObject.name} is Blue!");
        moveDirection = - (Camera.main.transform.position - transform.position);
        RB = GetComponent<Rigidbody>();
        RB.useGravity = false;
    } 

    public void OnDisable()
    {
        //Debug.Log($"{gameObject.name} is no longer Blue!");
        RB.useGravity = true;
    }

    void Update()
    {
        transform.position += moveDirection.normalized * Time.deltaTime * speed;
    }
}
