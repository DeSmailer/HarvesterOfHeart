using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Boss2Enemy : MonoBehaviour
{
    //хп
    public float currentHP;
    public float maxHP;
    public Slider slider;
    //ищет игрока
    private GameObject Player;
    //чем стреляет
    public GameObject projectile;
    //простая атака
    public Transform attackPos;
    public float startTimeBtwAttac;
    public float timeBtwAttac = 2;
    //павер атака
    public float startTimeBtwPowerAttac;
    private float timeBtwPowerAttac = 4;
    //время перед выстрелами павер атаки
    public float startTimeBtwPowerAttacShoot;
    private float timeBtwPowerAttacShoot = 0;
    //количество выстрелов в павер атаке
    public int startShootCount;
    private int shootCount;
    //что дропает
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
    public AudioClip[] clips;
    AudioSource audioSource;
    //анимации
    public Animator anim;
    void Start()
    {
        maxHP = currentHP = 40 * (LevelGenerator.LVL + LevelGenerator.LVL / 3);
        DisplayHP();
        shootCount = startShootCount;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
           


    void Update()
    {
        if (timeBtwAttac <= 0)
        {
            audioSource.clip = clips[0];
            print(audioSource.clip);
            audioSource.Play();
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
                    audioSource.clip = clips[0];
                    print(audioSource.clip);
                    audioSource.Play();
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
        DisplayHP();
        if (currentHP <= 0)
        {
            AmuletBuff.countDeadMobs += 5;

            for (int i = 0; i < 5; i++)
            {
                Drop();
            }

            anim.SetInteger("state", 3);
            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();

            RIP();

        }
    }

    void Drop()
    {
        Vector3 itemDropPos;
        float r = (int)Random.Range(0f, 4f);

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

