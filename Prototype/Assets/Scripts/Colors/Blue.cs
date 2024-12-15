using UnityEditor;
using UnityEngine;

public class Blue : MonoBehaviour
{
   [SerializeField] public float speed = 2f;
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
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
