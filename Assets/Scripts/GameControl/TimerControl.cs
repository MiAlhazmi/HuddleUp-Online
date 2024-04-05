using UnityEngine;
using UnityEngine.UI;

public class TimerControl : MonoBehaviour
{
    public CircularTimer[] circularTimers;
    public Text text;

    public void StartTimer()
    {
        text.text = "Start";
        foreach (CircularTimer timer in circularTimers)
        {
            timer.StartTimer();
        }
    }

    public void StartTimerNumber(int index)
    {
        circularTimers[index].StartTimer();
    }

    public void PauseTimer()
    {
        text.text = "Pause";
        foreach (CircularTimer timer in circularTimers)
        {
            timer.PauseTimer();
        }
    }

    public void StopTimer()
    {
        text.text = "Stop";
        foreach (CircularTimer timer in circularTimers)
        {
            timer.StopTimer();
        }
    }

    public void DidFinishedTimer()
    {
        text.text = "Finished";
        Debug.Log("Timer finished");
    }
}
