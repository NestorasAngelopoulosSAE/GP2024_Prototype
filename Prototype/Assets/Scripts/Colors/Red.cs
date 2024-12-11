using System;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Thamnopoulos Thanos 2024
/// 
///
/// </summary>

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
        float moveSpeed = 5.0f;
        transform.position = Vector3.Lerp(transform.position, holdArea.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation, Time.deltaTime * moveSpeed); ;
        

    }
}
