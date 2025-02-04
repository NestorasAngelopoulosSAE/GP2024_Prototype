/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Holds a list of available colors and the objects that the player has colored
/// Handles input and raycasting for coloring objects
/// </summary>
using UnityEngine;

[System.Serializable]
public struct GameplayColor
{
    public bool unlocked;
    public string name;
    public Color color;
    public KeyCode keybind;
    public GameObject coloredObject;

    public GameplayColor(Color Color = new Color())
    {
        unlocked = false;
        name = string.Empty;
        color = Color;
        keybind = KeyCode.None;
        coloredObject = null;
    }
}

public class ColorManager : MonoBehaviour
{
    [Tooltip("(Recommended)\nMakes it possible to paint over a previously painted surface, overriding its function.")]
    public bool overrideColors;

    [Tooltip("When enabled, blue movement is based on the orientation of the colorable object, and the position of the player with regards to it.\n\n(Recommended)\nWhen disabled, the object can only move towards the world space X and Z axis. The direction is determined not by the player's position, but the direction the player is facing.")]
    public bool allowBlueVertical;

    public int selectedColor = -1;
    
    public GameplayColor[] GameplayColors;

    [Tooltip("The color to be applied to objects when their color is removed.")]
    public Color defaultColorableColor;

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
                    brushAnimator.SetTrigger("Change");
                    break;
                }
            }
            return; // Don't bother checking for input if the player hasn't unlocked any color yet.
        }

        int prevSelection = selectedColor;
        // Switch selectedColor on scroll. (also handle overflow/underflow)
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && GameplayColors[selectedColor].unlocked) // forwards
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
        // Switch selectedColor with keybind.
        for (int i = 0; i < GameplayColors.Length; i++)
        {
            if (Input.GetKeyDown(GameplayColors[i].keybind) && GameplayColors[i].unlocked) selectedColor = i;
        }
        if (selectedColor != prevSelection) brushAnimator.SetTrigger("Change");

        //Color object when left click is presesd.
        if (Input.GetMouseButtonDown(0))
        {
            brushAnimator.SetTrigger("Apply");

            // If colorable, change the color of the object the player is looking at.
            LayerMask raycastLayers = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastLayers))
            {
                if (hit.collider.gameObject.tag == "Colorable")
                {
                    if (hit.collider.gameObject != GameplayColors[selectedColor].coloredObject)
                    {
                        // Clear any color that the hit object may have.
                        for (int i = 0; i < GameplayColors.Length; i++)
                        {
                            if (hit.collider.gameObject == GameplayColors[i].coloredObject)
                            {
                                if (!overrideColors) return;
                                RemoveColor(i, false, hit.point);
                            }
                        }
                        // Check if another object already has this color and if so clear it.
                        if (GameplayColors[selectedColor].coloredObject != null)
                        {
                            if (!overrideColors) return;
                            RemoveColor(selectedColor, true);
                        }
                    }

                    // Update the coloredObject and change its color.
                    GameplayColors[selectedColor].coloredObject = hit.collider.gameObject;
                    GameplayColors[selectedColor].coloredObject.GetComponent<Colorable>().SetColor(GameplayColors[selectedColor], true, hit.point);
                    // If the object has a timer script, start its timer.
                    if (GameplayColors[selectedColor].coloredObject.GetComponent<ScriptRemovalTimer>()) GameplayColors[selectedColor].coloredObject.GetComponent<ScriptRemovalTimer>().StartTimer();
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Remove color from object when right click is presesd.
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity) && hit.collider.gameObject == GameplayColors[selectedColor].coloredObject)
            {
                // If the player's looking at the coloredObject, start clearing the color from there.
                RemoveColor(selectedColor, true, hit.point);
            }
            else RemoveColor(selectedColor, true);
            brushAnimator.SetTrigger("Remove");
        }
    }

    /// <summary>
    /// Removes the specified colored object from the index. If clearColor is false, it doesn't clear its color.
    /// </summary>
    /// <param name="index">The index in GameplayColors of the color you want to remove.</param>
    /// <param name="clearColor">The point from which to start the discoloration.</param>
    public void RemoveColor(int index, bool clearColor)
    {
        if (GameplayColors[index].coloredObject == null) return;

        // Get the closest point on the surface of its collider to the camera, and start clearing from there.
        GameObject coloredObject = GameplayColors[index].coloredObject;
        Vector3 closestPoint = Physics.ClosestPoint(Camera.main.transform.position, coloredObject.GetComponent<Collider>(), coloredObject.transform.position, coloredObject.transform.rotation);
        RemoveColor(index, clearColor, closestPoint);
    }

    /// <summary>
    /// Removes the specified colored object from the index. If clearColor is false, it doesn't clear its color. The defaultColor will start at hitPoint and expand from there.
    /// </summary>
    /// <param name="index">The index in GameplayColors of the color you want to remove.</param>
    /// <param name="clearColor">If the color should be turned back to the defaultColorableColor.</param>
    /// <param name="hitPoint">The point from which to start the discoloration.</param>
    void RemoveColor(int index, bool clearColor, Vector3 hitPoint) // Clear color from curently colored object.
    {
        if (GameplayColors[index].coloredObject == null) return;

        // Set object's color to the default color.
        GameplayColors[index].coloredObject.GetComponent<Colorable>().SetColor(new GameplayColor(defaultColorableColor), clearColor, hitPoint);
        GameplayColors[index].coloredObject = null; // Forget this object.
    }
}
