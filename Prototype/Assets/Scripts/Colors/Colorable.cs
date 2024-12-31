/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes object colorable. Dynamically adds and removes corresponding scripts for each color.
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

    void Start()
    {
        gameObject.tag = "Colorable";
        myRenderer = GetComponent<Renderer>();
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
    }

    private void Update()
    {
        threshold += Time.deltaTime * speed; 
        myRenderer.material.SetFloat("_Threshold", threshold);
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

        if (clearColor)
        {
            Color lastColor = myRenderer.material.GetColor("_InsideColor");
            myRenderer.material.SetColor("_InsideColor", newColor.color);
            myRenderer.material.SetColor("_OutsideColor", lastColor);
            myRenderer.material.SetVector("_HitPosition", transform.InverseTransformPoint(hitPosition));
            threshold = 0;
        }
    }
}
