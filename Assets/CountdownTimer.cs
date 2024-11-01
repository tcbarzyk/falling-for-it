using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float startTimeInSeconds = 60f;  // Set the initial countdown time in seconds
    public TextMeshProUGUI timerText;                // Assign a UI Text element to display the timer
    private float timeRemaining;
    private bool timerIsRunning = false;

    void Start()
    {
        timeRemaining = startTimeInSeconds;
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                // Timer has reached zero
                Debug.Log("Time's up!");
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayTime(timeRemaining);  // Final display to show "00:00.000"
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Get the minutes, seconds, and milliseconds
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int milliseconds = Mathf.FloorToInt((timeToDisplay * 1000) % 1000);

        // Format the time as MM:SS.mmm
        timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
