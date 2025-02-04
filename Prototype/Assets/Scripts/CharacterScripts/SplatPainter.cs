/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Generates textures for, and paints Splat Catchers with the selected color, based on a bitmap texture.
/// </summary>
using UnityEngine;
using UnityEngine.Audio;

public class Splat : MonoBehaviour
{
    ColorManager colorManager;
    UIManager uiManager;

    public Texture2D emptyTexture;
    public Texture2D brushTexture;
    public Material splatMaterial;

    [SerializeField] bool AllowColorMixing;

    [SerializeField] AudioMixerGroup SFXGroup;
    AudioSource audioSource;
    [SerializeField] AudioClip brushStrokeLoop;
    float pitch = 1;

    bool currentlyPainting;
    float targetVolume = 0.5f;

    void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        uiManager = GameObject.FindGameObjectWithTag("Gameplay Manager").GetComponent<UIManager>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXGroup;
        audioSource.clip = brushStrokeLoop;
        audioSource.loop = true;
        audioSource.volume = 0;
        audioSource.Play();
    }

    void Update()
    {
        if (uiManager.isPaused)
        {
            audioSource.volume = 0f;
            return;
        }

        if (Input.GetMouseButton(0) && colorManager.selectedColor != -1) PaintObject(colorManager.GameplayColors[colorManager.selectedColor].color);
        else if (Input.GetMouseButton(1)) PaintObject(Color.clear);

        // Change pitch up and down.
        float recenterMultiplier = 2;
        pitch += Random.Range(-0.5f, 0.5f) * Time.deltaTime * (1 + (Mathf.Abs(1 - pitch) * recenterMultiplier));
        pitch = Mathf.Clamp(pitch, 0.8f, 1.2f);
        audioSource.pitch = pitch;

        // Fade SFX in and out.
        if (currentlyPainting) audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, 4 * Time.deltaTime);
        else audioSource.volume = Mathf.Lerp(audioSource.volume, 0, 8 * Time.deltaTime);

        currentlyPainting = false;
    }

    void PaintObject(Color color)
    {
        // Mask out all layers to ignore.
        LayerMask raycastLayers = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, raycastLayers))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Splat"))
            {
                // Play brush sound.
                if (color != Color.clear) currentlyPainting = true;

                MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();

                // Get already applied texture.
                Texture2D tex = meshRenderer.material.GetTexture("_Splat_Texture") as Texture2D;
                if (tex == emptyTexture) // If a texture hasn't been created,
                {
                    if (color == Color.clear) return; // Return if this won't have any effect on the object.

                    // Create a new texture that's 2000 by 2000 pixels
                    tex = new Texture2D(2000, 2000);
                    Color[] canvas = new Color[2000 * 2000];

                    Debug.Log($"{tex.width} {tex.height}");

                    // Make it transparent.
                    for (int i = 0; i < canvas.Length; i++) canvas[i] = Color.clear;
                    tex.SetPixels(canvas);
                    // Make sure it's sharp and doesn't repeat at the edges.
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    meshRenderer.material.SetTexture("_Splat_Texture", tex);
                }

                // Get UV point corresponding to hit point.
                Vector2 center = hit.textureCoord;
                center.x *= tex.width;
                center.y *= tex.height;
                
                // Paint the pixels according to the bitmap provided.
                for (int i = 0; i < brushTexture.width; i++)
                {
                    for (int j = 0; j < brushTexture.height; j++)
                    {
                        int x = (int)(center.x - brushTexture.width / 2 + i);
                        int y = (int)(center.y - brushTexture.height / 2 + j);
                        Color bitmapColor = brushTexture.GetPixel(i, j);
                        if (bitmapColor == Color.black) continue;
                        
                        if (AllowColorMixing && color != Color.clear) tex.SetPixel(x, y, tex.GetPixel(x, y) + color * bitmapColor);
                        else tex.SetPixel(x, y, color * bitmapColor);
                    }
                }

                tex.Apply();
            }
        }
    }
}
