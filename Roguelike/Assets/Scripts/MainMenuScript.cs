using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnPlayClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}