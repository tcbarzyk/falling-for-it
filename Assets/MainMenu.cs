using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(float dangerValue)
    {
        Time.timeScale = 1f;
        DangerManager.Instance.Danger = dangerValue;
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Quit game");
    }
}
