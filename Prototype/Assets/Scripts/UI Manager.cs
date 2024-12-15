/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Manages all things UI
/// </summary>

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    bool isPaused;
    public GameObject PauseMenu;
    public GameObject Player;

    void Start()
    {
        PauseMenu.SetActive(false);
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
            Time.timeScale = 0f;
            PauseMenu.SetActive(true);
            Player.GetComponent<PlayerController>().enabled = false;
            GetComponent<ColorManager>().enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            PauseMenu.SetActive(false);
            Player.GetComponent<PlayerController>().enabled = true;
            GetComponent<ColorManager>().enabled = true;
        }
    }
}
