using UnityEngine;

public class Red : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"{gameObject.name} is Red!");
    }

    public void OnDisable()
    {
        Debug.Log($"{gameObject.name} is no longer Red!");
    }

    void Update()
    {
        
    }
}
