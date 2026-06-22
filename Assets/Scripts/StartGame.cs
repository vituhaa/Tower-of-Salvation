using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private string firstLevelName = "Story_start";

    public void Start_Game()
    {   
        PlayerPrefs.DeleteKey("LastLevel"); 
        SceneManager.LoadScene(firstLevelName);
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("LastLevel"))
        {
            string lastLevel = PlayerPrefs.GetString("LastLevel");
            SceneManager.LoadScene(lastLevel);
        }
        else
        {
            Start_Game();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}