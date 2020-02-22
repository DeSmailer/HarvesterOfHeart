using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterRestarter : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown("]"))
        {
            Restart();
        }
    }

    public void Restart()
    {
        LevelGenerator.LVL++;
        var objs = GameObject.FindGameObjectsWithTag("wall"); // возвращает МАССИВ!
        for (int i = 0; i < objs.Length; i++)
            Destroy(objs[i]);

        var objs2 = GameObject.FindGameObjectsWithTag("floor"); // возвращает МАССИВ!
        for (int i = 0; i < objs2.Length; i++)
            Destroy(objs2[i]);

        var objs3 = GameObject.FindGameObjectsWithTag("Portal"); // возвращает МАССИВ!
        for (int i = 0; i < objs3.Length; i++)
            Destroy(objs3[i]);

        var objs4 = GameObject.FindGameObjectsWithTag("HealthPotionItem"); // возвращает МАССИВ!
        for (int i = 0; i < objs4.Length; i++)
            Destroy(objs4[i]);

        var objs5 = GameObject.FindGameObjectsWithTag("ScrollItem"); // возвращает МАССИВ!
        for (int i = 0; i < objs5.Length; i++)
            Destroy(objs5[i]);

        var objs6 = GameObject.FindGameObjectsWithTag("SoulItem"); // возвращает МАССИВ!
        for (int i = 0; i < objs6.Length; i++)
            Destroy(objs6[i]);

        objs2 = GameObject.FindGameObjectsWithTag("floor"); // возвращает МАССИВ!
        for (int i = 0; i < objs2.Length; i++)
            Destroy(objs2[i]);

        objs2 = GameObject.FindGameObjectsWithTag("Enemy"); // возвращает МАССИВ!
        for (int i = 0; i < objs2.Length; i++)
            Destroy(objs2[i]);

        objs2 = GameObject.FindGameObjectsWithTag("AmuletItem"); // возвращает МАССИВ!
        for (int i = 0; i < objs2.Length; i++)
            Destroy(objs2[i]);

        objs2 = GameObject.FindGameObjectsWithTag("Menoreh"); // возвращает МАССИВ!
        for (int i = 0; i < objs2.Length; i++)
            Destroy(objs2[i]);

        GameObject go = GameObject.FindGameObjectWithTag("levelGenerator");
        go.GetComponent<LevelGenerator>().Start();
    }
}
