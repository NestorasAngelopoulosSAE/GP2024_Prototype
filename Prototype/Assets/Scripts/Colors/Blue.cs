using UnityEngine;

public class Blue : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"{gameObject.name} is Blue!");
    }

    public void OnDisable()
    {
        Debug.Log($"{gameObject.name} is no longer Blue!");
    }

    void Update()
    {
        
    }
}
