using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss3Enemy : MonoBehaviour
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
    private bool isMoving;
    //игрока ищет
    public GameObject Player;
    //урон
    private float damage;
    public LayerMask whatIsEnemies;
    //хп гг
    private PlayerHP playerHP;
    private bool isImpulse = false;
    //простая атака
    public float startTimeBtwAttac;
    public float timeBtwAttac = 0;
    public GameObject SpawnMob;
    //время перед сплеш атакой
    public float startTimeToSplashAttack;
    private float timeToSplashAttack;
    public GameObject splashAttackPositiont;
    //что дропает
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
    //анимации
    public Animator anim;
    private LevelGenerator levelGenerator;
    public AudioClip[] clips;
    AudioSource audioSource;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player != null)
        {
            PathFinder = GetComponent<PathFinder>();
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMoving = true;
        }
        damage = 3 * LevelGenerator.LVL;
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        maxHP = currentHP = 40 * (LevelGenerator.LVL + LevelGenerator.LVL / 3);
        speed = Random.Range(1f, 3f);
        DisplayHP();
        anim = GetComponent<Animator>();
        levelGenerator = GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {

        if (Player == null) return;
        //спавн мелкого моба
        if (timeBtwAttac <= 0)
        {
            Shoot();
            SetStartTime();
        }
        else
        {
            //anim.SetInteger("state", 0);
            timeBtwAttac -= Time.deltaTime;
        }
        //подходит на 2 и стоит
        if (Vector2.Distance(transform.position, Player.transform.position) > 2f)
        {
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMoving = true;
            if(transform.position.x< Player.transform.position.x)
            {
                anim.SetInteger("state", 2);
            }
            else
            {
                anim.SetInteger("state", 1);
            }  

        }
        else
        {
            anim.SetInteger("state", 0);
            isMoving = false;
        }


        //если ты под некром стоишь больше старт секунд и расстояние меньше 1.7 то отталкивает
        if (Vector2.Distance(transform.position, Player.transform.position) < 1.7f)
        {
            timeToSplashAttack += Time.deltaTime;
            if (timeToSplashAttack >= startTimeToSplashAttack)
            {
                audioSource.clip = clips[1];
                print(audioSource.clip);
                audioSource.Play();
                anim.SetInteger("state", 4);
            }
        }
        else
        {
            timeToSplashAttack = 0;
        }


        if (isMoving)
        {
            if (Vector2.Distance(transform.position, PathToPlayer[PathToPlayer.Count - 1]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToPlayer[PathToPlayer.Count - 1].x, PathToPlayer[PathToPlayer.Count - 1].y, -91), speed * Time.deltaTime);

            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMoving = true;
        }
    }
    public void Shoot()
    {
        audioSource.clip = clips[0];
        print(audioSource.clip);
        audioSource.Play();
        //полы от 3 до 11 во все стороны
        Instantiate(SpawnMob, new Vector3(Random.Range((int)3, (int)11), Random.Range((int)3, (int)11), -95), Quaternion.identity);
        levelGenerator.MobCountOnLvl++;
        //print(levelGenerator.MobCountOnLvl);
    }

    public void SplashShoot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (Vector2.Distance(splashAttackPositiont.transform.position, player.transform.position) < 2.5)
            player.transform.Translate(new Vector2(player.transform.position.x - splashAttackPositiont.transform.position.x, player.transform.position.y - splashAttackPositiont.transform.position.y) * 7 * Time.deltaTime);
        //Instantiate(womenBeamSplash, new Vector3(attackPositiont.transform.position.x, splashAttackPositiont.transform.position.y, -95), Quaternion.identity);
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
                Drop();
            }
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

