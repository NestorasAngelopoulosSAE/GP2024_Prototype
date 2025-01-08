/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Used on trigger objects enable the unlocking of new colors.
/// </summary>
using UnityEngine;

public class ColorUnlock : MonoBehaviour
{
    ColorManager colorManager;
    public int IndexOfColorToUnlock;

    private void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Unlock color.
            colorManager.GameplayColors[IndexOfColorToUnlock].unlocked = true;
            // Animate brush to switch to the new color.
            colorManager.selectedColor = IndexOfColorToUnlock;
            colorManager.brushAnimator.SetTrigger("Change");
            // Delete the trigger object.
            Destroy(gameObject);
        }
    }
}
