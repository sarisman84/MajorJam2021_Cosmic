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
        Stopwatch ??= new Stopwatch();
        Stopwatch.StartNew();
    }

    public void EndTimer()
    {
        Stopwatch.Stop();
        currentTime = $"Cur Time:{Stopwatch.Elapsed.Minutes} minutes and {Stopwatch.Elapsed.Seconds} seconds";
        Debug.Log(currentTime);
    }
}