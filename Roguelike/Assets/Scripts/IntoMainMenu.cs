using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntoMainMenu : MonoBehaviour
{
    //время перед перезапуском
    public float time = 4;

    private void Start()
    {
        LevelGenerator.LVL = 1;
        AmuletBuff.GdropCount = 0;
        AmuletBuff.BdropCount = 0;
        AmuletBuff.YdropCount = 0;
        AmuletBuff.countDeadMobs = 0;
        AmuletBuff.SetBuff(0, 0, 1);

    }
    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0)
        {
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}
