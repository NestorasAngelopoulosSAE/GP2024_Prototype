using UnityEngine;

public class Green : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"{gameObject.name} is Green!");
    }

    public void OnDisable()
    {
        Debug.Log($"{gameObject.name} is no longer Green!");
    }

    void Update()
    {
        
    }
}
