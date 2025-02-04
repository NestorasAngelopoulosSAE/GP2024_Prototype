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
    void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        ResetTimeScale();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResetTimeScale();
    }

    public void Load(string levelName)
    {
        Load(SceneManager.GetSceneByName(levelName).buildIndex);
    }

    public void Load(int level)
    {
        SceneManager.LoadScene(level);
        ResetTimeScale();
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ResetTimeScale();
    }

    public void LoadPrevious()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        ResetTimeScale();
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
