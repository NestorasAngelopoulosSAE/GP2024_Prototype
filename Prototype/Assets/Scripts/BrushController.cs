/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Changes the brush texture and updates the soft bone refference object for correct physics on the brush tip.
/// </summary>

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BrushController : MonoBehaviour
{
    [SerializeField] Image Crosshair;
    [SerializeField] Texture defaultTexture;
    [SerializeField] Texture[] Textures;
    [SerializeField] GameObject softBoneRefference;
    
    ColorManager colorManager;
    SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] AudioMixerGroup SFXGroup;
    AudioSource audioSource;
    [SerializeField] AudioClip[] brushWhooshes;

    private void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXGroup;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;

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

    public void PlayWhooshSound(float basePitch)
    {
        if (brushWhooshes.Length > 0)
        {
            audioSource.clip = brushWhooshes[Random.Range(0, brushWhooshes.Length)];
            audioSource.pitch = basePitch + Random.Range(-0.05f, 0.05f);
            audioSource.Play();
        }
        else Debug.LogError("No brush sounds in brush controller.");
    }
}
