/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Holds a list of available colors and the objects that the player has colored
/// Handles input and raycasting for coloring objects
/// </summary>

using UnityEditor.ShaderGraph;
using UnityEngine;

[System.Serializable]
public struct GameplayColor
{
    public bool unlocked;
    public KeyCode keybind;
    public Material material;
    public GameObject coloredObject;
}

public class ColorManager : MonoBehaviour
{
    [Tooltip("(Recommended)\nMakes it possible to paint over a previously painted surface, overriding its function.")]
    public bool overrideColors;

    [Tooltip("When enabled, blue movement is based on the orientation of the colorable object, and the position of the player with regards to it.\n\n(Recommended)\nWhen disabled, the object can only move towards the world space X and Z axis. The direction is determined not by the player's position, but the direction the player is facing.")]
    public bool allowBlueVertical;

    public int selectedColor;
    
    public GameplayColor[] GameplayColors;

    [Tooltip("The material to be applied to objects when their color is removed.")]
    public Material colorableMaterial;

    public Animator brushAnimator;

    UIManager uiManager;

    private void Start()
    {
        uiManager = GetComponent<UIManager>();
        
        // Begin the level with the first unlocked color on the brush. If no color is unlocked, leave the brush dry.
        selectedColor = -1;
        for (int i = 0; i < GameplayColors.Length; i++)
        {
            if (GameplayColors[i].unlocked)
            {
                selectedColor = i;
                brushAnimator.gameObject.GetComponent<BrushController>().ChangeColor();
                break;
            }
        }
    }

    void Update()
    {
        if (uiManager.isPaused) return; // Don't take input if the game is paused.

        if (selectedColor == -1)
        {
            // Check if a color was unlocked.
            for (int i = 0; i < GameplayColors.Length; i++)
            {
                if (GameplayColors[i].unlocked)
                {
                    selectedColor = i;
                    brushAnimator.gameObject.GetComponent<BrushController>().ChangeColor();
                    break;
                }
            }
            return; // Don't bother checking for input if the player hasn't unlocked any color yet.
        }

        int prevSelection = selectedColor;
        // Switch selectedColor on scroll (also handle overflow/underflow)
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && GameplayColors[selectedColor].unlocked) // forward
        {
            do
            {
                selectedColor++;
                if (selectedColor >= GameplayColors.Length) selectedColor = 0;
            }
            while (!GameplayColors[selectedColor].unlocked);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && GameplayColors[selectedColor].unlocked) // backwards
        {
            do
            {
                selectedColor--;
                if (selectedColor < 0) selectedColor = GameplayColors.Length - 1;
            }
            while (!GameplayColors[selectedColor].unlocked);
        }
        // Switch selectedColor with keybind
        for (int i = 0; i < GameplayColors.Length; i++)
        {
            if (Input.GetKeyDown(GameplayColors[i].keybind) && GameplayColors[i].unlocked) selectedColor = i;
        }
        if (selectedColor != prevSelection) brushAnimator.SetTrigger("Change");

        //Color object when left click is presesd
        if (Input.GetMouseButtonDown(0))
        {
            brushAnimator.SetTrigger("Apply");

            // If colorable, change the color of the object the player is looking at
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "Colorable")
                {
                    if (hit.collider.gameObject != GameplayColors[selectedColor].coloredObject)
                    {
                        // Clear any color that the hit object may have
                        for (int i = 0; i < GameplayColors.Length; i++)
                        {
                            if (hit.collider.gameObject == GameplayColors[i].coloredObject)
                            {
                                if (!overrideColors) return;
                                RemoveColor(i);
                            }
                        }
                        // Check if another object already has this color and if so clear it
                        if (GameplayColors[selectedColor].coloredObject != null)
                        {
                            if (!overrideColors) return;
                            RemoveColor(selectedColor);
                        }
                    }

                    GameplayColors[selectedColor].coloredObject = hit.collider.gameObject;
                    GameplayColors[selectedColor].coloredObject.GetComponent<Colorable>().SetColor(GameplayColors[selectedColor]);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemoveColor(selectedColor); // Remove color from object when right click is presesd
            brushAnimator.SetTrigger("Remove");
        }
    }

    void RemoveColor(int index) // Clear color from curently colored object
    {
        if (GameplayColors[index].coloredObject == null) return;

        // Set object's material to Unity's default material and forget it
        GameplayColor defaultColor = new GameplayColor();
        if (colorableMaterial != null) defaultColor.material = colorableMaterial;
        else defaultColor.material = new Material(Shader.Find("Diffuse"));
        GameplayColors[index].coloredObject.GetComponent<Colorable>().SetColor(defaultColor);
        GameplayColors[index].coloredObject = null;        
    }
}
