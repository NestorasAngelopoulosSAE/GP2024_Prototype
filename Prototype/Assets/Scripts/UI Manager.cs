/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Manages all things UI
/// </summary>

using EZhex1991.EZSoftBone;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool isPaused { get; private set; }
    public GameObject PauseMenu;
    public GameObject Player;

    [Tooltip("A float to save the current EZSoftBone ConstantDeltaTime for the brush hairs so that it can be retrieved when resuming.")]
    float brushPhysicsDeltaTime;

    void Start()
    {
        PauseMenu.SetActive(false);
        brushPhysicsDeltaTime = Player.GetComponentInChildren<EZSoftBone>().constantDeltaTime;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            // Redundant due to Time Scale
            //Player.GetComponentInChildren<Animator>().enabled = false; // Stop brush animation
            //brushPhysicsDeltaTime = Player.GetComponentInChildren<EZSoftBone>().constantDeltaTime;
            //Player.GetComponentInChildren<EZSoftBone>().constantDeltaTime = 0f; // Stop brush physics

            Time.timeScale = 0f;

            PauseMenu.SetActive(true);
            Player.GetComponent<PlayerController>().enabled = false;
            GetComponent<ColorManager>().enabled = false;
        }
        else
        {
            // Redundant due to Time Scale
            //Player.GetComponentInChildren<Animator>().enabled = true; // Resume brush Animation
            //Player.GetComponentInChildren<EZSoftBone>().constantDeltaTime = brushPhysicsDeltaTime; // Resume brush Physics

            Time.timeScale = 1f;

            PauseMenu.SetActive(false);
            Player.GetComponent<PlayerController>().enabled = true;
            GetComponent<ColorManager>().enabled = true;
        }
    }
}
