/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Holds a list of available colors and the objects that the player has colored
/// Handles input and raycasting for coloring objects
/// </summary>

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GameplayColor
{
    public Material material;
    public GameObject coloredObject;
}

public class ColorManager : MonoBehaviour
{
    public int selectedColor;
    
    public GameplayColor[] GameplayColors;

    List<KeyCode> numberKeys = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
    };

    void Update()
    {
        // Switch selectedColor on scroll (also handle overflow/underflow)
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            selectedColor++;
            if (selectedColor >= GameplayColors.Length) selectedColor = 0;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            selectedColor--;
            if (selectedColor < 0) selectedColor = GameplayColors.Length - 1;
        }
        // Switch selectedColor on number press (1,2,3)
        foreach (var key in numberKeys) 
        {
            if (Input.GetKeyDown(key)) selectedColor = numberKeys.IndexOf(key);
        }
        
        //Color object when left click is presesd
        if (Input.GetMouseButtonDown(0))
        {
            // Shoot a ray of infinite length forward and change its color if it's colorable
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Colorable")
                {
                    // Clear any color that the hit object may have
                    for (int i = 0; i < GameplayColors.Length; i++)
                    {
                        if (hit.collider.gameObject == GameplayColors[i].coloredObject) RemoveColor(i);
                    }
                    // Check if another object already has this color and if so clear it
                    if (GameplayColors[selectedColor].coloredObject != null && hit.collider.gameObject != GameplayColors[selectedColor].coloredObject)
                    {
                        RemoveColor(selectedColor);
                    }

                    GameplayColors[selectedColor].coloredObject = hit.collider.gameObject;
                    GameplayColors[selectedColor].coloredObject.GetComponent<Colorable>().SetColor(GameplayColors[selectedColor]);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) RemoveColor(selectedColor); // Remove color from object when right click is presesd
    }

    void RemoveColor(int index) // Clear color from curently colored object
    {
        if (GameplayColors[index].coloredObject == null) return;

        // Set object's material to Unity's default material and forget it
        GameplayColor defaultColor = new GameplayColor();
        defaultColor.material = new Material(Shader.Find("Diffuse"));
        GameplayColors[index].coloredObject.GetComponent<Colorable>().SetColor(defaultColor);
        GameplayColors[index].coloredObject = null;
    }
}
