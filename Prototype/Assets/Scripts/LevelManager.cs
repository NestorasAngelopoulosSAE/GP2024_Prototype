/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Interface to handle loading levels.
/// </summary>
using UnityEngine.SceneManagement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Load(string levelName)
    {
        Load(SceneManager.GetSceneByName(levelName).buildIndex);
    }

    public void Load(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPrevious()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
