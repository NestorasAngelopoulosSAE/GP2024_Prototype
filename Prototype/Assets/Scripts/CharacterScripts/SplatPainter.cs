/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Manages the "Splat" texture of the toon shader, allowing the player to paint the walls.
/// Meshes must have 
/// </summary>
using UnityEngine;

public class Splat : MonoBehaviour
{
    ColorManager colorManager;
    UIManager uiManager;

    public int brushSize;

    void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        uiManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<UIManager>();
    }

    void Update()
    {
        if (uiManager.isPaused) return;

        if (Input.GetMouseButton(0)) PaintObject(colorManager.GameplayColors[colorManager.selectedColor].color);
        else if (Input.GetMouseButton(1)) PaintObject(Color.white);
    }

    void PaintObject(Color color)
    {
        // If colorable, change the color of the object the player is looking at.
        LayerMask raycastLayers = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastLayers))
        {
            if (hit.collider.tag != "Colorable" && hit.collider.tag != "Door" && hit.collider.tag != "Button")
            {
                MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();

                // Get already applied texture.
                Texture2D tex = meshRenderer.material.GetTexture("_Splat_Texture") as Texture2D;
                if (!tex) // If a texture hasn't been created,
                {
                    if (color == Color.white) return; // Return if this won't have any effect on the object.

                    // Create a new texture that's 100 by 100 pixels times the average scale of the object.
                    int objectScale = (int)(hit.transform.localScale.x + hit.transform.localScale.y + hit.transform.localScale.z) / 3;
                    tex = new Texture2D(100 * objectScale, 100 * objectScale);
                    Color[] canvas = new Color[100 * 100 * (int)Mathf.Pow(objectScale, 2)];

                    // Set it to plain white.
                    for (int i = 0; i < canvas.Length; i++) canvas[i] = Color.white;
                    tex.SetPixels(canvas);
                    tex.filterMode = FilterMode.Point;
                    meshRenderer.material.SetTexture("_Splat_Texture", tex);
                }

                // Get UV point corresponding to hit point.
                Vector2 center = hit.textureCoord;

                // Paint cross of pixels.
                tex.SetPixel((int)(center.x * tex.width), (int)(center.y * tex.height), color);
                tex.SetPixel((int)(center.x * tex.width) + 1, (int)(center.y * tex.height), color);
                tex.SetPixel((int)(center.x * tex.width), (int)(center.y * tex.height) + 1, color);
                tex.SetPixel((int)(center.x * tex.width) - 1, (int)(center.y * tex.height), color);
                tex.SetPixel((int)(center.x * tex.width), (int)(center.y * tex.height) - 1, color);

                tex.Apply();
            }
        }
    }
}
