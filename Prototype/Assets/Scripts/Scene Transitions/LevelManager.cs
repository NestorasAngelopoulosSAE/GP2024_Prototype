/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Interface to handle loading levels using the wipe transition.
/// </summary>
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public Material WipeMaterial;
    readonly int _circleSizeId = Shader.PropertyToID("_Circle_Size");

    float wipeSpeed = 5f;
    float waitTime = 0.5f; // Time to to hold the black screen for.

    bool transitionStarted;
    bool transitionComplete;

    private void Awake() => WipeMaterial.SetFloat(_circleSizeId, 0f);

    private void Update()
    {
        if (Time.timeSinceLevelLoad <= waitTime / 2) return;

        float size = WipeMaterial.GetFloat(_circleSizeId);

        if (!transitionStarted) WipeMaterial.SetFloat(_circleSizeId, Mathf.MoveTowards(size, 1.5f, wipeSpeed * Time.unscaledDeltaTime));
        else
        {
            WipeMaterial.SetFloat(_circleSizeId, Mathf.MoveTowards(size, 0, wipeSpeed * Time.unscaledDeltaTime));
            if (size <= 0f) transitionComplete = true;
        }
    }
    
    public void Reload() => Load(SceneManager.GetActiveScene().buildIndex);
    public void LoadMainMenu() => Load(0);
    public void LoadNext() => Load(SceneManager.GetActiveScene().buildIndex + 1);
    public void LoadPrevious() => Load(SceneManager.GetActiveScene().buildIndex - 1);
    public void Load(string levelName) => Load(SceneManager.GetSceneByName(levelName).buildIndex);
    public void Load(int level) => StartCoroutine(Transition(level));

    IEnumerator Transition(int level)
    {
        transitionStarted = true;

        while (!transitionComplete) yield return null;
        yield return new WaitForSecondsRealtime(waitTime / 2);

        SceneManager.LoadScene(level);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
