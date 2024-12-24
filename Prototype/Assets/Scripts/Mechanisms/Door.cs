/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// Slides object between its open and closed positions.
/// </summary>
using UnityEngine;

public class Door : MonoBehaviour
{
    // This script's inspector is overriden by the editor script "DoorEditor.cs"
    public bool isOpen;
    public Vector3 closedPosition;
    public Vector3 openPosition;
    public float speed = 1f;

    private void Reset() // Set default positions to describe door motion when applying component to object
    {
        closedPosition = transform.position;
        openPosition = transform.position + transform.up * transform.localScale.y;
    }

    private void Update()
    {
        if (isOpen) transform.position = Vector3.MoveTowards(transform.position, openPosition, Time.deltaTime * speed);
        else transform.position = Vector3.MoveTowards(transform.position, closedPosition, Time.deltaTime * speed);
    }

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }

    public void Toggle()
    {
        isOpen = !isOpen;
    }
}
