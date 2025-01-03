using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainButtonContainer;
    public GameObject SettingsButtonContainer;
    public GameObject AudioSettingsContainer;
    public void MainMenu()
    {
        SettingsButtonContainer.SetActive(false);
        MainButtonContainer.SetActive(true);
    }
    public void Settings()
    {
        MainButtonContainer.SetActive(false);
        SettingsButtonContainer.SetActive(true);

    }

    public void AudioSettings()
    {
        SettingsButtonContainer.SetActive(false);
        AudioSettingsContainer.SetActive(true);
    }



}
