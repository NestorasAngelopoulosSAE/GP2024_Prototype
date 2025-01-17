/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Generates textures for and paints Splat Catchers with the selected color.
/// </summary>
using UnityEngine;

public class Splat : MonoBehaviour
{
    ColorManager colorManager;
    UIManager uiManager;

    public Texture2D emptyTexture;
    public Material splatMaterial;

    void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        uiManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<UIManager>();

        // Make sure the shader renders properly for the Player.
        splatMaterial.SetFloat(Shader.PropertyToID("_ZTest"), (int)UnityEngine.Rendering.CompareFunction.LessEqual);
        //splatMaterial.SetFloat(Shader.PropertyToID("_Cull"), (int)UnityEngine.Rendering.CullMode.Back);
        if (splatMaterial.GetInt("_VISUALIZE") == 1) wasVisualised = true;
        splatMaterial.SetInt("_VISUALIZE", 0);
    }

    bool wasVisualised;

    private void OnDisable()
    {
        // Reset shader properties for in-editor visualization.
        splatMaterial.SetFloat(Shader.PropertyToID("_ZTest"), (int)UnityEngine.Rendering.CompareFunction.Always);
        //splatMaterial.SetFloat(Shader.PropertyToID("_Cull"), (int)UnityEngine.Rendering.CullMode.Off);
        if (wasVisualised) splatMaterial.SetInt("_VISUALIZE", 1);
    }

    void Update()
    {
        if (uiManager.isPaused) return;

        if (Input.GetMouseButton(0) && colorManager.selectedColor != -1) PaintObject(colorManager.GameplayColors[colorManager.selectedColor].color);
        else if (Input.GetMouseButton(1)) PaintObject(Color.clear);
    }

    void PaintObject(Color color)
    {
        // Mask out all layers to ignore.
        LayerMask raycastLayers = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastLayers))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Splat"))
            {
                MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();

                // Get already applied texture.
                Texture2D tex = meshRenderer.material.GetTexture("_Splat_Texture") as Texture2D;
                if (tex == emptyTexture) // If a texture hasn't been created,
                {
                    if (color == Color.clear) return; // Return if this won't have any effect on the object.

                    // Create a new texture that's 1000 by 1000 pixels
                    tex = new Texture2D(1000, 1000);
                    Color[] canvas = new Color[1000 * 1000];

                    // Set it to plain white.
                    for (int i = 0; i < canvas.Length; i++) canvas[i] = Color.clear;
                    tex.SetPixels(canvas);
                    // Make sure it's sharp and doesn't repeat at the edges.
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    meshRenderer.material.SetTexture("_Splat_Texture", tex);
                }

                // Get UV point corresponding to hit point.
                Vector2 center = hit.textureCoord;
                int x = (int)(center.x * tex.width);
                int y = (int)(center.y * tex.height);

                // Paint cross of pixels.
                tex.SetPixel(x, y, color);
                tex.SetPixel(x + 1, y, color);
                tex.SetPixel(x, y + 1, color);
                tex.SetPixel(x - 1, y, color);
                tex.SetPixel(x, y - 1, color);

                // Make erasing have a bigger brush.
                if (color == Color.clear)
                {
                    tex.SetPixel(x + 1, y + 1, color);
                    tex.SetPixel(x + 1, y - 1, color);
                    tex.SetPixel(x - 1, y + 1, color);
                    tex.SetPixel(x - 1, y - 1, color);

                    tex.SetPixel(x + 2, y, color);
                    tex.SetPixel(x, y + 2, color);
                    tex.SetPixel(x - 2, y, color);
                    tex.SetPixel(x, y - 2, color);
                }

                tex.Apply();
            }
        }
    }
}
