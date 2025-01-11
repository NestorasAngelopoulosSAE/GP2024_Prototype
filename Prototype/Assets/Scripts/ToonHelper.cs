/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Updates the object's toon shader with the information of the closest non-main light.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class ToonHelper : MonoBehaviour
{
    public List<Light> sceneLights = new List<Light>();
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        foreach (Light light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            sceneLights.Add(light);
        }
    }

    void Update()
    {
        // Get the closest light that isn't the directional light.
        float minDistance = -1;
        Light closestLight = null;
        foreach (Light light in sceneLights)
        {
            float distance = Vector3.Distance(transform.position, light.transform.position);
            if ((minDistance == -1 || distance < minDistance) && light.type != LightType.Directional)
            {
                minDistance = distance;
                closestLight = light;
            }
        }

        if (closestLight)
        {
            // Pass the direction and color of the closest light to the shader.
            meshRenderer.material.SetVector("_Secondary_Light_Direction", (transform.position - closestLight.transform.position).normalized);

            // Calculate the falloff in light intensity and darken the color accordingly. 
            float attenuation = 1 / Vector3.Distance(transform.position, closestLight.transform.position);
            Color secondaryColor = closestLight.color * closestLight.intensity * Mathf.Clamp01(attenuation);
            secondaryColor.a = attenuation;
            meshRenderer.material.SetColor("_Secondary_Light_Color", secondaryColor);
        }
        else meshRenderer.material.SetColor("_Secondary_Light_Color", Color.black);
    }
}
