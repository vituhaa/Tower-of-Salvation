using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "Start";
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}