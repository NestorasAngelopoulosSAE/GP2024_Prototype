/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Is used as a middleman between buttons and doors when multiple buttons need to be pressed to open the door.
/// </summary>
using UnityEngine;
using UnityEngine.Events;

public class SignalAccumulator : MonoBehaviour
{
    public int signalThreshold;
    int signals;
    bool reached;
    bool lost;

    public UnityEvent onSignalsReached;
    public UnityEvent onSignalsLost;

    void Update()
    {
        if (signals >= signalThreshold &! reached)
        {
            onSignalsReached.Invoke();
            reached = true;
            lost = false;
        }
        
        if (signals < signalThreshold &! lost)
        {
            onSignalsLost.Invoke();
            reached = false;
            lost = true;
        }
    }

    public void Add()
    {
        signals++;
    }

    public void Subtract()
    {
        signals--;
    }
}
