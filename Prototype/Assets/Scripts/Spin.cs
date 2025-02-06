using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    Vector3 basePos;
    [SerializeField] float rotationSpeed = 1;
    [SerializeField] float bobHeight;
    [SerializeField] float bobSpeed = 1;

    void Start()
    {
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.InverseTransformVector(Vector3.up), rotationSpeed * Time.deltaTime);

        transform.position = basePos + new Vector3(0, Mathf.Sin(Time.time * bobSpeed) * bobHeight, 0);
    }
}
