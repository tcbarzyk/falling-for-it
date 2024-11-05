using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float startTimeInSeconds = 60f;  // Set the initial countdown time in seconds
    public TextMeshProUGUI timerText;  // Assign a UI Text element to display the timer
    private float timeRemaining;
    private bool timerIsRunning = false;
    public PlayerCombat player;
    public Image timerBackground;

    private Color originalBackgroundColor;
    private bool isFlashing = false;

    void Start()
    {
        timeRemaining = startTimeInSeconds;
        timerIsRunning = true;
        originalBackgroundColor = timerBackground.color;  // Store the original color of the background
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

                // Start flashing if time remaining is less than 15 seconds
                if (timeRemaining <= 15f && !isFlashing)
                {
                    StartCoroutine(FlashBackground());
                }
            }
            else
            {
                // Timer has reached zero
                Debug.Log("Time's up!");
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayTime(timeRemaining);  // Final display to show "00:00.000"
                player.gameOverOutOfTime();
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

    private IEnumerator FlashBackground()
    {
        isFlashing = true;

        while (timeRemaining > 0 && timeRemaining <= 15f)
        {
            timerBackground.color = Color.red;  // Set the background color to red
            yield return new WaitForSeconds(0.5f);  // Wait for half a second
            timerBackground.color = originalBackgroundColor;  // Revert to the original color
            yield return new WaitForSeconds(0.5f);  // Wait for another half a second
        }

        // Stop flashing and reset the color when time goes above 15 seconds (if needed)
        timerBackground.color = originalBackgroundColor;
        isFlashing = false;
    }
}
