using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Image Logo;

    private void Start()
    {
        // Demonstration of the new logo: (you can remove this or use this feature differently if you like)
        // picks the color of the logo randomly between red, green and blue.
        Logo.color = new Color[] { Color.red, Color.green, Color.blue }[Random.Range(0, 3)];
    }

    // Nestoras: these aren't necessary since you're changing the ui through the button events.
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
