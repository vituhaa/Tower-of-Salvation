using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save : MonoBehaviour
{
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "Start" && currentScene != "Win" && 
            currentScene != "Story_start" &&
            currentScene != "Story_end")
        {
            PlayerPrefs.SetString("LastLevel", currentScene);
            PlayerPrefs.Save();
        }
    }
}
