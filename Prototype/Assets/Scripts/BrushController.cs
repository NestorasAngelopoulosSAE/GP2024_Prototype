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
    public GameObject softBoneRefference;
    
    ColorManager colorManager;
    SkinnedMeshRenderer skinnedMeshRenderer;

    private void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        if (!Crosshair) Crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        ChangeColor();
    }

    void Update()
    {
        softBoneRefference.transform.rotation = Quaternion.identity; // Stabilizes the Brush Soft Bone Refference Object so that the hairs of the brush move naturally based on the player's movement.
    }

    public void ChangeColor()
    {
        skinnedMeshRenderer.material.SetTexture("_Texture", Textures[colorManager.selectedColor]); // Set brush color
        Crosshair.color = colorManager.GameplayColors[colorManager.selectedColor].color; // Set crosshair color
    }

    private void OnApplicationQuit()
    {
        skinnedMeshRenderer.material.SetTexture("_Texture", defaultTexture); // Reset texture to be blank when not in playmode
    }
}
