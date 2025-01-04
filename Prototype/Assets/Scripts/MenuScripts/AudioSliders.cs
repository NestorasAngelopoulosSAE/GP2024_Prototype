using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSliders : MonoBehaviour
{

    public AudioMixer masterMixer;

    public void SetSfxLvl(float sfxLvl)
    {
        masterMixer.SetFloat("SFXVolume", sfxLvl);
    }
   
    public void MusicLvl(float musicLvl)
    {
        masterMixer.SetFloat("MusicVolume", musicLvl);
    }

    public void MasterLvl(float masterLvl)
    {
        masterMixer.SetFloat("MasterVolume", masterLvl);
    }
}
