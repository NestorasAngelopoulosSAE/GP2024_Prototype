using UnityEngine;
using UnityEngine.Events;

public class ScriptRemovalTimer : MonoBehaviour
{

    public float removalTime = 4f;
    private float timer = 0f;
    private bool isTimerActive = false;

    //Unity Event Vars...
    public UnityEvent WaxOn;
    public UnityEvent WaxOff;

    ColorManager colorManager;

    public void Start()
    {
        colorManager = GameObject.FindWithTag("Gameplay Manager").GetComponent<ColorManager>();

        if (WaxOn == null)
            WaxOn = new UnityEvent();

        if (WaxOff == null)
            WaxOff = new UnityEvent();
    }

    public void StartTimer()
    {
        timer = 0f;
        isTimerActive = true;

        WaxOn.Invoke();
    }

    private void Update()
    {
        if (isTimerActive)
        {
            timer += Time.deltaTime;

            if (timer >= removalTime)
            {
                RemoveColor();
                isTimerActive = false;
            }
        }
    }

    private void RemoveColor()
    {
        WaxOff.Invoke();

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script is Red)
            {
                colorManager.RemoveColor(0, true);
                Debug.Log("Lamao no colouri no mor");
            }
            else if (script is Green)
            {
                colorManager.RemoveColor(1, true);
                Debug.Log("Lamao no colouri no mor");
            }
            else if (script is Blue)
            {
                colorManager.RemoveColor(2, true);
                Debug.Log("Lamao no colouri no mor");
            }
        }
    }
}