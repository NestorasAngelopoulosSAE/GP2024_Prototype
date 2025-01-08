/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes object colorable. Dynamically adds and removes corresponding scripts for each color.
/// Adds a RideTrigger object so that objects in a stack all follow each other.
/// Controls the shader that swipes each color over the object.
/// </summary>
using System;
using UnityEngine;

public class Colorable : MonoBehaviour
{
    ColorManager colorManager;
    Renderer myRenderer;

    float threshold;
    float speed = 5;

    Vector3 SpawnPosition;
    Quaternion SpawnRotation;
    Rigidbody rigidbody;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();

        gameObject.tag = "Colorable";
        // Add a ride trigger so that a stack of objects can all move along with a moving platform.
        CreateRideTriggerObject();

        if (GetComponent<ScriptRemovalTimer>()) myRenderer.material.SetInt("_IsTimer", 1);


        rigidbody = GetComponent<Rigidbody>();
        SpawnPosition = transform.position;
        SpawnRotation = transform.rotation;
    }

    private void Update()
    {
        threshold += Time.deltaTime * speed; 
        myRenderer.material.SetFloat("_Threshold", threshold);


        // Respawn object if it fell off the map.
        if (transform.position.y < -10)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            transform.position = SpawnPosition;
            transform.rotation = SpawnRotation;
        }
    }

    public void SetColor(GameplayColor newColor, bool clearColor, Vector3 hitPosition)
    {
        foreach (GameplayColor gameplayColor in colorManager.GameplayColors) // Set the correct components for the object's new color.
        {
            if (gameplayColor.Equals(newColor))
            {
                if (GetComponent(Type.GetType(gameplayColor.name)) == null) gameObject.AddComponent(Type.GetType(gameplayColor.name));
            }
            else Destroy(GetComponent(Type.GetType(gameplayColor.name)));
        }

        if (clearColor) // Update the shader.
        {
            Color lastColor = myRenderer.material.GetColor("_InsideColor");
            myRenderer.material.SetColor("_InsideColor", newColor.color);
            myRenderer.material.SetColor("_OutsideColor", lastColor);
            myRenderer.material.SetVector("_HitPosition", transform.InverseTransformPoint(hitPosition));
            threshold = 0;
        }
    }
    
    GameObject CreateRideTriggerObject()
    {
        GameObject RideTriggerGameObject = new GameObject("Ride Trigger");
        RideTriggerGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        RideTriggerGameObject.AddComponent<RideTrigger>();

        // Make new object follow its source object.
        RideTriggerGameObject.transform.parent = transform;

        // Copy Collider over to trigger object;
        Collider triggerCollider = Blue.CopyCollider(GetComponent<Collider>(), RideTriggerGameObject);
        triggerCollider.isTrigger = true;

        return RideTriggerGameObject;
    }
}
