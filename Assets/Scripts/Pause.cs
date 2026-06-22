using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] private GameObject panel;
    [SerializeField] private string mainMenuSceneName = "Start";

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        AudioListener.pause = true;
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        AudioListener.pause = false;
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitButton()
    {
        ResumeGame();
        Application.Quit();
    }
}