using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    private Animator anim;
    private bool isOpened = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isOpened = true;

            StartCoroutine(OpenDoorAndLoadNextLevel());
        }
    }

    private IEnumerator OpenDoorAndLoadNextLevel()
    {
        anim.SetTrigger("DoorOpenTrigger");

        yield return new WaitForSeconds(0.5f);

        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentLevelIndex + 1);
    }
}
