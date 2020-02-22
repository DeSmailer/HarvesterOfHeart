using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletBuff : MonoBehaviour
{
    public static float GdropCount = 0;
    public static float BdropCount = 0;
    public static float YdropCount = 0;
    public static float countDeadMobs = 0;
    public static void SetBuff(float hpBuff, float damaheBuff, float ratioBuff)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerHP>().HPBuff(hpBuff);
        player.GetComponent<PlayerHP>().RatioBuff(ratioBuff);
        player.GetComponent<PlayerAttack>().DamageBuff(damaheBuff);
    }
}
