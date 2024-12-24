/// <summary>
/// Nestoras Angelopoulos 2024
/// 
/// An editor script with gizmos for setting the path of a sliding door while in edit mode.
/// </summary>
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    Door door;
    Tool lastTool;
    bool drawHandles;
    bool isHidden;
    bool isShown;

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector(); // We override the default inspector of the door script.

        door = (Door)target; // Get the script whose values we're displaying

        door.speed = EditorGUILayout.FloatField("Open / Close Speed", door.speed); // Display the speed parameter

        // Syncronization in line 109 breaks the door when rapid calls to toggle its state occure while looking at the door's inspector
        if (Application.isPlaying) GUI.enabled = false;

        // Display 4 buttons next to each other with presets for the opening direction of the door
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Slide Up")) door.openPosition = door.transform.position + door.transform.up * door.transform.localScale.y;
        if (GUILayout.Button("Slide Down")) door.openPosition = door.transform.position - door.transform.up * door.transform.localScale.y;
        if (GUILayout.Button("Slide Left")) door.openPosition = door.transform.position - door.transform.right * door.transform.localScale.x;
        if (GUILayout.Button("Slide Right")) door.openPosition = door.transform.position + door.transform.right * door.transform.localScale.x;
        GUILayout.EndHorizontal();

        // Only show the relevant properties.
        if (door.isOpen)
        {
            door.closedPosition = EditorGUILayout.Vector3Field("Closed Position", door.closedPosition);

            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                door.isOpen = false;
                if (!Application.isPlaying) door.transform.position = door.closedPosition;
            }
        }
        else
        {
            door.openPosition = EditorGUILayout.Vector3Field("Open Position", door.openPosition);

            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open"))
            {
                door.isOpen = true;
                if (!Application.isPlaying) door.transform.position = door.openPosition;
            }
        }

        if (Application.isPlaying) GUI.enabled = false;

        // Display button to toggle position handle gizmos in the scene view
        if (GUILayout.Button($"{(drawHandles ? "Hide" : "Show")} Handles"))
        {
            drawHandles = !drawHandles;
        }
        GUILayout.EndHorizontal();

        if (GUI.changed) SceneView.RepaintAll(); // Update the scene view
    }

    private void OnSceneGUI()
    {
        if (door == null) return;

        // Draw a dotted line showing the path of the door in the scene view.
        Handles.color = Color.magenta;
        Handles.DrawDottedLine(door.closedPosition, door.openPosition, 4f);

        if (drawHandles)
        {
            // Hide the default widget.
            if (!isHidden) // This only runs once after the toggle.
            {
                isHidden = true;
                isShown = false;
                lastTool = Tools.current;
                Tools.current = Tool.None;
            }

            // Draw labels and handles for each important position.
            Handles.Label(door.openPosition, "\n Open Position");
            Handles.Label(door.closedPosition, "\n Closed Position");
            door.openPosition = Handles.PositionHandle(door.openPosition, Quaternion.identity);
            door.closedPosition = Handles.PositionHandle(door.closedPosition, Quaternion.identity);
        }
        else if (!isShown) // Return to the last used tool.
        {
            isShown = true;
            isHidden = false;
            Tools.current = lastTool;
        }

        // Syncronize the position of the handle with the position of the door.
        // May break door if focused on it during play mode. Therefore, handles are disabled during play mode. (line 27, 63)
        if (Application.isPlaying) return;
        if (GUI.changed)
        {
            if (door.isOpen) door.transform.position = door.openPosition;
            else door.transform.position = door.closedPosition;
        }
        else
        {
            if (door.isOpen) door.openPosition = door.transform.position;
            else door.closedPosition = door.transform.position;
        }
    }

    private void OnEnable()
    {
        lastTool = Tools.current;
    }

    private void OnDisable()
    {
        if (drawHandles) Tools.current = lastTool; // Reset tool even if user forgets to hide the handles.
    }
}