using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ControllsSettings : MonoBehaviour
{
   public PlayerController playerController;
   public Slider SensitivitySlider; 
   public void SetRotationSpeed(float SensSpeed)
   {
        if (playerController) playerController.rotationSpeed = SensSpeed;
        PlayerPrefs.SetFloat("Sensitivity", SensSpeed);
   }

   public void Start()
   {
       if (PlayerPrefs.HasKey("Sensitivity"))
       {
            SensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
       }
   }

}
