using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public AudioSource gameOverSound;
    public AudioSource gameMusic;
    public AudioSource gameOverMusic;
    public void Setup()
    {
        gameOverSound.Play();
        gameMusic.Stop();
        gameOverMusic.Play();
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
