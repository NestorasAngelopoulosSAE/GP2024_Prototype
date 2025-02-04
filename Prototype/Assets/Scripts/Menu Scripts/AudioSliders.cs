using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{

    public AudioMixer masterMixer;
    public Slider SFXSlider;
    public Slider MasterSlider;
    public Slider MusicSlider;

    public void SetSfxLvl(float sfxLvl)
    {
        masterMixer.SetFloat("SFXVolume", sfxLvl);
        PlayerPrefs.SetFloat("sfxVolume", sfxLvl);
    }
   
    public void MusicLvl(float musicLvl)
    {
        masterMixer.SetFloat("MusicVolume", musicLvl);
        PlayerPrefs.SetFloat("MusicVolume", musicLvl);
    }

    public void MasterLvl(float masterLvl)
    {
        masterMixer.SetFloat("MasterVolume", masterLvl);
        PlayerPrefs.SetFloat("MasterVolume", masterLvl);
    }

    public void Start()
    {   // Saving values

        //SFX
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            // set slider to player prefs value 
            SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            // set mixer volume to player prefs value 
            masterMixer.SetFloat("sfxVolume", SFXSlider.value);            
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            // set slider to player prefs value 
            MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            // set mixer volume to player prefs value 
            masterMixer.SetFloat("MasterVolume", MasterSlider.value);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            // set slider to player prefs value 
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            // set mixer volume to player prefs value 
            masterMixer.SetFloat("MusicVolume", MusicSlider.value);
        }
    }
}
