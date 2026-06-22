using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyStart : MonoBehaviour
{
    [SerializeField] private string levelName = "Level_1";

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
