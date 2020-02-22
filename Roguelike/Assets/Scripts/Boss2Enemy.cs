﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Boss2Enemy : MonoBehaviour
{
    public float currentHP;
    public float maxHP;

    //путь
    private List<Vector2> PathToPlayer = new List<Vector2>();
    private GameObject Player;

    //атака
    public Transform attackPos;

    public float startTimeBtwAttac;
    public float timeBtwAttac = 2;

    public float startTimeBtwPowerAttac;
    private float timeBtwPowerAttac = 4;
    public float startTimeBtwPowerAttacShoot;
    private float timeBtwPowerAttacShoot = 0;
    public int startShootCount;
    private int shootCount;
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
    public Slider slider;

    public GameObject projectile;

    public Animator anim;
    void Start()
    {
        maxHP = currentHP = 40 * (LevelGenerator.LVL + LevelGenerator.LVL / 3);
        DisplayHP();
        shootCount = startShootCount;
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (timeBtwAttac <= 0)
        {
            print("da");
            anim.SetInteger("state", 1);
            Shoot();
            SetStartTime();
        }
        else
        {
            timeBtwAttac -= Time.deltaTime;
            anim.SetInteger("state", 1);

        }
        if (timeBtwPowerAttac <= 0)
        {
            if (shootCount > 0)
            {
                if (timeBtwPowerAttacShoot <= 0)
                {
                    anim.SetInteger("state", 2);
                }
                else
                {
                    anim.SetInteger("state", 0);
                    timeBtwPowerAttacShoot -= Time.deltaTime;
                }

            }
            else
            {
                shootCount = startShootCount;
                anim.SetInteger("state", 0);
                timeBtwPowerAttac = startTimeBtwPowerAttac;
            }
        }
        else
        {
            anim.SetInteger("state", 0);
            timeBtwPowerAttac -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        print("Fay-Fay");
        DisplayHP();
        if (currentHP <= 0)
        {
            AmuletBuff.countDeadMobs += 5;

            for (int i = 0; i < 5; i++)
            {
                Vector3 itemDropPos;
                int r = (int)Random.Range(0f, 4f);

                if (r == 0)
                {
                    itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                    Instantiate(HealthPotion, itemDropPos, Quaternion.identity);
                }

                else if (r == 1)
                {
                    itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                    Instantiate(Scroll, itemDropPos, Quaternion.identity);
                }

                else if (r == 2)
                {
                    itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                    Instantiate(Soull, itemDropPos, Quaternion.identity);
                }

                else if (r == 3)
                {
                    itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                    Instantiate(HealthPotion, itemDropPos, Quaternion.identity);
                }
            }
            Vector3 itemDropPos1;
            float r1 = Random.Range(0f, 1f);
            if (r1 <= DropAmuletChance(3, AmuletBuff.GdropCount, AmuletBuff.countDeadMobs))
            {
                itemDropPos1 = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                Instantiate(GAmulet, itemDropPos1, Quaternion.identity);
                AmuletBuff.GdropCount++;
            }
            r1 = Random.Range(0f, 1f);
            if (r1 <= DropAmuletChance(2, AmuletBuff.BdropCount, AmuletBuff.countDeadMobs))
            {
                itemDropPos1 = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                Instantiate(BAmulet, itemDropPos1, Quaternion.identity);
                AmuletBuff.BdropCount++;
            }
            r1 = Random.Range(0f, 1f);
            if (r1 <= DropAmuletChance(2, AmuletBuff.YdropCount, AmuletBuff.countDeadMobs))
            {
                itemDropPos1 = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
                Instantiate(YAmulet, itemDropPos1, Quaternion.identity);
                AmuletBuff.YdropCount++;
            }

            anim.SetInteger("state", 3);
            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();

            RIP();

        }
    }
    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
    {
        return ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
    }
    private void DisplayHP()
    {
        float HPSlider = currentHP / maxHP;
        slider.value = HPSlider;
    }

    void Shoot()
    {
        Instantiate(projectile, attackPos.transform.position, Quaternion.AngleAxis(CalculateAngle(), Vector3.forward));
    }

    private float CalculateAngle()
    {
        Vector2 Player = GameObject.FindWithTag("Player").transform.position;

        Vector2 dir = Player - (Vector2)attackPos.transform.position;

        return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public void SetStartTime()
    {
        timeBtwAttac = startTimeBtwAttac;
    }
    public void SetStartTime2()
    {
        shootCount--;
        print(shootCount);
        timeBtwPowerAttacShoot = startTimeBtwPowerAttacShoot;
    }

    public void RIP()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }
   
}
