/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Manages all things UI
/// </summary>

using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool isPaused { get; private set; }
    public GameObject PauseMenu;
    public GameObject Player;

    void Start()
    {
        PauseMenu.SetActive(false);
        if (isPaused)
        {
            TogglePause();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;

            PauseMenu.SetActive(true);
            Player.GetComponent<PlayerController>().enabled = false;
            GetComponent<ColorManager>().enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;

            PauseMenu.SetActive(false);
            Player.GetComponent<PlayerController>().enabled = true;
            GetComponent<ColorManager>().enabled = true;
        }
    }
}
