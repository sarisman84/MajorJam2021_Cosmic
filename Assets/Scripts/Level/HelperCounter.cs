using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class HelperCounter : MonoBehaviour
{
    [SerializeField] private string currentTime;
    private Stopwatch Stopwatch;

    public void StartTimer()
    {
        currentTime = "";


        if (Stopwatch == null)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }
        else
        {
            Stopwatch.Restart();
        }
    }

    public void EndTimer()
    {
        currentTime = $"Cur Time:{Stopwatch.Elapsed.Minutes} minutes and {Stopwatch.Elapsed.Seconds} seconds";
        Stopwatch.Stop();
        Debug.Log(currentTime);
    }
}