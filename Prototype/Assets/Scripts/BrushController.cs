/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Changes the brush texture and updates the soft bone refference object for correct physics on the brush tip.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

public class BrushController : MonoBehaviour
{
    public Image Crosshair;
    public Texture defaultTexture;
    public Texture[] Textures;
    public Material brushMaterial;
    public GameObject softBoneRefference;
    
    ColorManager colorManager;

    private void Start()
    {
        colorManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<ColorManager>();
        if (!Crosshair) Crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        //ChangeColor();
    }

    void Update()
    {
        softBoneRefference.transform.rotation = Quaternion.identity; // Stabilizes the Brush Soft Bone Refference Object so that the hairs of the brush move naturally based on the player's movement.
    }

    public void ChangeColor()
    {
        brushMaterial.mainTexture = Textures[colorManager.selectedColor]; // Set brush color
        Crosshair.color = colorManager.GameplayColors[colorManager.selectedColor].material.color; // Set crosshair color
    }

    private void OnApplicationQuit()
    {
         brushMaterial.mainTexture = defaultTexture; // Reset texture to be blank when not in playmode
    }
}
