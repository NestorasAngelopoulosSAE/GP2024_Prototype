/// <summary>
/// Thamnopoulos Thanos 2024
/// Makes objects holdable
/// Red : )
/// </summary>

using UnityEngine;

public class Red : MonoBehaviour
{
    // Assign Refs...

    Transform holdArea;
    
    private Rigidbody thisRB;
    private float Mass;
    bool isHeld;
       
    private float pickupRange = 2.0f;
    

    void Start()
    {
        holdArea = GameObject.Find("Range").transform;

        thisRB = GetComponent<Rigidbody>();
       
        Debug.Log($"{gameObject.name} is Red!");
    }

    public void OnDisable()
    {
        Debug.Log($"{gameObject.name} is no longer Red!");
        DropObject();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHeld)
            {
                DropObject();
            }
            else
            {
                // Checking if player is looking at the object with a Ray...
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    if (hit.transform.gameObject == gameObject)
                    {                      
                        PickupObject();
                    }
                }

            }
        }
    }

    private void FixedUpdate()
    { 
        if (isHeld == true) 
        {
            Move();
        }
    }

    private void PickupObject()
    {
        Mass = thisRB.mass; // getting starting mass.
        thisRB.mass = Mathf.Epsilon; // changing said mass to Epsilon.
        isHeld = true;
        thisRB.constraints = RigidbodyConstraints.FreezeRotation;
        thisRB.useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("Held Object");
    }

    private void DropObject()
    {
        thisRB.mass = Mass;
        isHeld = false;
        thisRB.constraints = RigidbodyConstraints.None;
        thisRB.useGravity = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, holdArea.position) > 5f) DropObject();
        float moveSpeed = 5.0f;

        // Nestoras Cameo: Raycasts to the next position (ignoring the player and this object) and moves to the collision point instead of the holdArea. This avoids clipping and jittering.
        if (Physics.Raycast(transform.position, holdArea.position - transform.position, out RaycastHit hit, Vector3.Distance(transform.position, holdArea.position), LayerMask.NameToLayer("Held Object") | LayerMask.NameToLayer("Player")))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * moveSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, holdArea.position, Time.deltaTime * moveSpeed);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation, Time.deltaTime * moveSpeed);
    }
}
