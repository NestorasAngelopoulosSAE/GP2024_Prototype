/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes object colorable. Dynamically adds and removes corresponding scripts for each color.
/// Adds a RideTrigger object so that objects in a stack all follow each other.
/// Controls the shader that swipes each color over the object.
/// </summary>
using System;
using UnityEngine;
using UnityEngine.Audio;

public class Colorable : MonoBehaviour
{
    ColorManager colorManager;
    AudioSource audioSource;
    [SerializeField] AudioMixerGroup SFXGroup;
    [SerializeField] AudioClip respawnSound;
    Renderer meshRenderer;
    int _Threshold = Shader.PropertyToID("_Threshold");
    int _InsideColor = Shader.PropertyToID("_Inside_Color");
    int _OusideColor = Shader.PropertyToID("_Outside_Color");
    int _HitPosition = Shader.PropertyToID("_Hit_Position");

    float threshold;
    float speed = 5;

    Vector3 SpawnPosition;
    Quaternion SpawnRotation;
    Rigidbody rb;

    bool startKinematic;

    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXGroup;
        audioSource.clip = respawnSound;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1;

        gameObject.tag = "Colorable";
        // Add a ride trigger so that a stack of objects can all move along with a moving platform.
        if (!GetComponentInChildren<RideTrigger>()) CreateRideTriggerObject();
        // Give object a helper script to manage the secondary light interactions with the toon shader.
        if (!GetComponent<ToonHelper>()) gameObject.AddComponent<ToonHelper>();

        if (GetComponent<ScriptRemovalTimer>()) meshRenderer.material.SetInt("_Is_Timer", 1);

        rb = GetComponent<Rigidbody>();
        SpawnPosition = transform.position;
        SpawnRotation = transform.rotation;
        startKinematic = rb.isKinematic;
    }

    private void Update()
    {
        // Respawn object if it fell off the map.
        if (transform.position.y < -53f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = startKinematic;

            transform.position = SpawnPosition;
            transform.rotation = SpawnRotation;
            audioSource.Play();
        }
    }

    private void LateUpdate()
    {
        threshold += Time.deltaTime * speed;
        meshRenderer.material.SetFloat(_Threshold, threshold);
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
            Color lastColor = meshRenderer.material.GetColor(_InsideColor);
            meshRenderer.material.SetColor(_InsideColor, newColor.color);
            meshRenderer.material.SetColor(_OusideColor, lastColor);
            meshRenderer.material.SetVector(_HitPosition, transform.InverseTransformPoint(hitPosition));
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
