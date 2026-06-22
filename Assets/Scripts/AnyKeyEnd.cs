using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyEnd : MonoBehaviour
{
    [SerializeField] private string levelName = "Win";

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
