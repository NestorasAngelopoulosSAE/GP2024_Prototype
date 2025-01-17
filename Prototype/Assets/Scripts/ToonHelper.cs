/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Updates the object's toon shader with the information of the closest non-main light.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class ToonHelper : MonoBehaviour
{
    static readonly int m_SecondaryLightPositionId = Shader.PropertyToID("_Secondary_Light_Position");
    static readonly int m_SecondaryLightColorId = Shader.PropertyToID("_Secondary_Light_Color");
    
    public List<Light> sceneLights = new List<Light>();
    MeshRenderer meshRenderer;
    SkinnedMeshRenderer skinnedMeshRenderer;


    void Start()
    {
        if (GetComponent<MeshRenderer>()) meshRenderer = GetComponent<MeshRenderer>();
        else skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

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
            // Pass the position and color of the closest light to the shader.
            if (meshRenderer) meshRenderer.material.SetVector(m_SecondaryLightPositionId, closestLight.transform.position);
            else skinnedMeshRenderer.material.SetVector(m_SecondaryLightPositionId, closestLight.transform.position);

            Color secondaryColor = closestLight.color;
            secondaryColor.a = closestLight.intensity;
            if (meshRenderer) meshRenderer.material.SetColor(m_SecondaryLightColorId, secondaryColor);
            else skinnedMeshRenderer.material.SetColor(m_SecondaryLightColorId, secondaryColor);
        }
        else
        {
            if (meshRenderer) meshRenderer.material.SetColor(m_SecondaryLightColorId, Color.black);
            else skinnedMeshRenderer.material.SetColor(m_SecondaryLightColorId, Color.black);
        }
    }
}
