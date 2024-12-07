/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Makes object colorable. Dynamically adds and removes corresponding scripts for each color.
/// </summary>

using System;
using UnityEngine;

public class Colorable : MonoBehaviour
{
    ColorManager colorManager;
    Renderer myRenderer;

    void Start()
    {
        gameObject.tag = "Colorable";
        myRenderer = GetComponent<Renderer>();
        colorManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<ColorManager>();
    }

    public void SetColor(GameplayColor newColor)
    {
        myRenderer.material = newColor.material;
        foreach (GameplayColor color in colorManager.GameplayColors) // Set the correct components for the object's new color
        {
            if (color.Equals(newColor))
            {
                if (GetComponent(Type.GetType(color.material.name)) == null) gameObject.AddComponent(Type.GetType(color.material.name));
            }
            else Destroy(GetComponent(Type.GetType(color.material.name)));
        }
    }
}
