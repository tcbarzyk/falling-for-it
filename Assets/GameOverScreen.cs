using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public void Setup()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
