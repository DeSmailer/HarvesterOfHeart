using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemy : MonoBehaviour
{
    //хп
    public float health;

    //путь
    public float speed;
    private List<Vector2> PathToTarget = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMooving;
    private GameObject Player;
    //атака
    [Header("урон(коефициент перед умножением на лвл)")]
    public float damage;
    private PlayerHP playerHP;
    public Transform attackPos;
    public float attackRange;
    public float startTimeBtwAttac;
    private float timeBtwAttac = 0;
    //кого бить
    public LayerMask whatIsEnemies;
    //анимации
    public Animator anim;
    //пепел после смерти
    public GameObject deathEffect;
    public AudioClip[] clips;
    AudioSource audioSource;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            PathFinder = GetComponent<PathFinder>();
            isMooving = true;
        }

        damage = damage * LevelGenerator.LVL;
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        health = 7.8f * LevelGenerator.LVL;
        speed = UnityEngine.Random.Range(1f, 2f);

        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {

        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player == null) return;
        else
        {
            Move(Player.transform.position, true);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            AmuletBuff.countDeadMobs++;
            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
            RIP();
        }
    }

   

    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
    {
        float res = ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
        return res;
    }

    private void Move(Vector2 target, bool player)
    {
        //если видит игрока
        if (player)
        {
            if (Vector2.Distance(transform.position, Player.transform.position) > attackRange * 7 / 7.1f)
            {
                PathToTarget = PathFinder.GetPath(Player.transform.position);

                isMooving = true;
                if (Player.transform.position.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (Player.transform.position.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }

            }
            else
            {
                isMooving = false;
                anim.SetInteger("state", 0);
            }

            //атака
            //print("PathToTarget.Count " + PathToTarget.Count);
            if (PathToTarget.Count == 0)
            {
                if (timeBtwAttac <= 0)
                {
                    audioSource.clip = clips[0];
                    print(audioSource.clip);
                    audioSource.Play();
                    if (Player.transform.position.x - transform.position.x < 0)
                    {
                        anim.SetInteger("state", 4);
                    }
                    else if (Player.transform.position.x - transform.position.x >= 0)
                    {
                        anim.SetInteger("state", 3);
                    }
                }
                else
                {
                    anim.SetInteger("state", 0);
                    timeBtwAttac -= Time.deltaTime;
                }

            }

            if (isMooving)
            {
                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);
                }
                else
                {
                    anim.SetInteger("state", 0);
                    isMooving = false;
                }
            }
            else
            {
                PathToTarget = PathFinder.GetPath(Player.transform.position);
                isMooving = true;
            }
        }
    }

    public void Attack()
    {
        Collider2D PlayerToDamage = Physics2D.OverlapCircle(attackPos.position, attackRange, whatIsEnemies);
        if (PlayerToDamage != null)
        {
            playerHP.TakingDamage(damage);
            timeBtwAttac = startTimeBtwAttac;
        }

    }

    public void SetStartTime()
    {
        timeBtwAttac = startTimeBtwAttac;
    }

    public void RIP()
    {
        Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y, -50), Quaternion.identity);
        Destroy(gameObject);
    }
}
