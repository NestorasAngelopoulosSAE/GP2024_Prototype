/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Used on trigger objects enable the unlocking of new colors.
/// </summary>
using UnityEngine;
using UnityEngine.Events;

public class ColorUnlock : MonoBehaviour
{
    ColorManager colorManager;
    public int IndexOfColorToUnlock;
    [SerializeField] Light MainLight;
    [SerializeField] Light PedestalLight;
    [SerializeField] float lightTransitionSpeed = 1;
    float targetIntensity;

    public UnityEvent onUnlock;

    private void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
        targetIntensity = MainLight.intensity;
        MainLight.intensity = 0.2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" &! colorManager.GameplayColors[IndexOfColorToUnlock].unlocked)
        {
            // Unlock color.
            colorManager.GameplayColors[IndexOfColorToUnlock].unlocked = true;
            // Animate brush to switch to the new color.
            colorManager.selectedColor = IndexOfColorToUnlock;
            colorManager.brushAnimator.SetTrigger("Change");
            PedestalLight.color = Color.black;
            onUnlock.Invoke();
        }
    }

    private void Update()
    {
        if (colorManager.GameplayColors[IndexOfColorToUnlock].unlocked)
        {
            MainLight.intensity = Mathf.MoveTowards(MainLight.intensity, targetIntensity, lightTransitionSpeed * Time.deltaTime);
        }

        if (MainLight.intensity == targetIntensity) Destroy(gameObject);
    }
}
