using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Enemy : MonoBehaviour
{
    //движение
    public float speed;
    //хп
    public float currentHP;
    public float maxHP;
    public Slider slider;
    //путь
    private List<Vector2> PathToPlayer = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMooving;
    //игрока ищет
    public GameObject Player;
    //урон
    private float damage;
    public LayerMask whatIsEnemies;
    //хп гг
    private PlayerHP playerHP;
    //простая атака
    public float startTimeBtwAttac;
    public float timeBtwAttac = 0;
    public float offset;
    public GameObject womenBeam;
    //время перед сплеш атакой
    public float startTimeToSplashAttack;
    private float timeToSplashAttack;
    public GameObject womenBeamSplash;
    public GameObject attackPositiont;
    //что дропает
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
    //анимации
    public Animator anim;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player != null)
        {
            PathFinder = GetComponent<PathFinder>();
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMooving = true;
        }
        damage = 3 * LevelGenerator.LVL;
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        maxHP = currentHP = 40 *( LevelGenerator.LVL + LevelGenerator.LVL/3);
        speed = Random.Range(1f, 3f);
        DisplayHP();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        

        if (Player == null) return;
        //удар выстрел лучем 
        if (timeBtwAttac <= 0)
        {
            anim.SetInteger("state", 1);
        }
        else
        {
            anim.SetInteger("state", 0);
            timeBtwAttac -= Time.deltaTime;
        }
        //подходит на 4 и стоит
        if (Vector2.Distance(transform.position, Player.transform.position) > 1.5f) 
        {
            PathToPlayer = PathFinder.GetPath(Player.transform.position);

            isMooving = true;

        }
        else
        {
            isMooving = false;
        }


        //если ты под ангелком стоишь больше старт секунд и расстояние меньше 1.5 то на голову сплеш
        if (Vector2.Distance(transform.position, Player.transform.position) < 0.7f)
        {
            timeToSplashAttack += Time.deltaTime;
            if (timeToSplashAttack >= startTimeToSplashAttack)
            {

                anim.SetInteger("state", 2);
                timeToSplashAttack = 0;
            }
        }
        else
        {
            timeToSplashAttack = 0;
        }


        if (isMooving)
        {
            if (Vector2.Distance(transform.position, PathToPlayer[PathToPlayer.Count - 1]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToPlayer[PathToPlayer.Count - 1].x, PathToPlayer[PathToPlayer.Count - 1].y, -91), speed * Time.deltaTime);

            }
            else
            {
                isMooving = false;
            }
        }
        else
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMooving = true;
        }


    }
    void Shoot()
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("Player").transform;
        Instantiate(womenBeam,new Vector3( attackPos.transform.position.x, attackPos.transform.position.y + offset, -95), Quaternion.identity);
    }

    void SplashShoot()
    {
        Instantiate(womenBeamSplash, new Vector3(attackPositiont.transform.position.x, attackPositiont.transform.position.y, -95), Quaternion.identity);
    }
    
    private void DisplayHP()
    {
        float HPSlider = currentHP / maxHP;
        slider.value = HPSlider;
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

            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
            RIP();
        }
    }
    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
    {
        return ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
    }

    public void RIP()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }

    public void SetStartTime()
    {
        timeBtwAttac = startTimeBtwAttac;
    }
}
