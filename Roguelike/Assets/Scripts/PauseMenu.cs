using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool pause;
    public GameObject background;
    private void Start()
    {
        pause = false;
        background.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                OnPauseClick();
            }
            else
            {
                OnUnPauseClick();
            }
        }
    }

    public void OnPauseClick()
    {
        Time.timeScale = 0f;
        background.SetActive(true);
        pause = true;
    }
    public void OnUnPauseClick()
    {
        Time.timeScale = 1f;
        background.SetActive(false);
        pause = false;
    }
    public void IntoMainMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
