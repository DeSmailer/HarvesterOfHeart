
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //хп
    public float health;

    //путь
    public float speed;
    private List<Vector2> PathToTarget = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMooving;
    private GameObject Player;
    //позиции между которыми ходит 
    public Vector2 movePos1;
    public Vector2 movePos2;
    float[] p1p2 = new float[2];
    //время перед началом движения когда гг вышел из радиуса видимости
    public float timeToMove = 3;
    bool go1 = true;
    //атака
    [Header ("урон(коефициент перед умножением на лвл)")]
    public float damage;
    private PlayerHP playerHP;
    public Transform attackPos;
    public float attackRange;
    public float startTimeBtwAttac;
    public float timeBtwAttac = 0;
    //кого бить
    public LayerMask whatIsEnemies;
    //чо дропает
    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
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


            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2, 2);
            var position = transform.position;

            position.x = p1p2[0];
            position.y = p1p2[1];
            movePos1 = position;

            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2, 2);
            position.x = p1p2[0];
            position.y = p1p2[1];
            movePos2 = position;
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
        float dis = Vector2.Distance(transform.position, Player.transform.position);
        if (Vector2.Distance(transform.position, Player.transform.position) < 3f)
        {
            timeToMove = 3f;
            Move(Player.transform.position, true);
        }
        if (Vector2.Distance(transform.position, Player.transform.position) >= 3f)
        {
            if (timeToMove <= 0)
            {
                if (go1)
                {
                    Move(movePos1, false);
                    if (Vector2.Distance(transform.position, movePos1) <= 1.5f)
                    {
                        go1 = false;
                        timeToMove = 3f;
                        anim.SetInteger("state", 0);
                    }
                }
                else
                {
                    Move(movePos2, false);
                    if (Vector2.Distance(transform.position, movePos2) <= 1.5f)
                    {
                        go1 = true;
                        timeToMove = 3f;
                        anim.SetInteger("state", 0);
                    }
                }
            }
            else
            {
                timeToMove -= Time.deltaTime;
            }


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

            Drop();

            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
            RIP();
        }
    }

    private void Drop()
    {
        float r = (int)UnityEngine.Random.Range(0f, 4f);

        if (r == 0)
        {
            Instantiate(HealthPotion, transform.position, Quaternion.identity);
        }

        else if (r == 1)
        {
            Instantiate(Scroll, transform.position, Quaternion.identity);
        }

        else if (r == 2)
        {
            Instantiate(Soull, transform.position, Quaternion.identity);
        }

        else if (r == 3)
        {
            Instantiate(HealthPotion, transform.position, Quaternion.identity);
        }

        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(3, AmuletBuff.GdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(GAmulet, transform.position, Quaternion.identity);
            AmuletBuff.GdropCount++;
        }
        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(2, AmuletBuff.BdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(BAmulet, transform.position, Quaternion.identity);
            AmuletBuff.BdropCount++;
        }
        r = UnityEngine.Random.Range(0f, 1f);
        if (r <= DropAmuletChance(2, AmuletBuff.YdropCount, AmuletBuff.countDeadMobs))
        {
            Instantiate(YAmulet, transform.position, Quaternion.identity);
            AmuletBuff.YdropCount++;
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
            if (Vector2.Distance(transform.position, Player.transform.position) > attackRange)
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
            if (PathToTarget.Count == 0 || PathToTarget.Count == 1)
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
            else
            {
                timeBtwAttac -= Time.deltaTime;
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

        //если НЕ видит игрока
        else
        {
            if (Vector2.Distance(transform.position, target) > 0.2f)
            {
                PathToTarget = PathFinder.GetPath(target);

                isMooving = true;
                if (target.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (target.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }
            }
            if (PathToTarget.Count == 0)
            {
                return;
            }

            if (isMooving)
            {
                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);
                    if (target.x - transform.position.x < 0)
                    {
                        anim.SetInteger("state", 2);

                    }
                    else if (target.x - transform.position.x >= 0)
                    {
                        anim.SetInteger("state", 1);
                    }
                    isMooving = true;
                }
                else
                {
                    isMooving = false;
                    anim.SetInteger("state", 0);
                }
            }
            else
            {
                PathToTarget = PathFinder.GetPath(target);
                if (target.x - transform.position.x < 0)
                {
                    anim.SetInteger("state", 2);

                }
                else if (target.x - transform.position.x >= 0)
                {
                    anim.SetInteger("state", 1);
                }
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

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy : MonoBehaviour
//{
//    public float health;

//    //путь
//    private List<Vector2> PathToTarget = new List<Vector2>();
//    private PathFinder PathFinder;
//    private bool isMooving;
//    private GameObject Player;
//    public GameObject movePos1;
//    public GameObject movePos2;

//    public float timeToMove = 3;
//    bool go1 = true;

//    public float speed;


//    //атака
//    private float damage;
//    private PlayerHP playerHP;
//    public Transform attackPos;
//    public float attackRange;
//    public float startTimeBtwAttac;
//    private float timeBtwAttac = 0;
//    public LayerMask whatIsEnemies;
//    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
//    float[] p1p2 = new float[2];
//    void Start()
//    {
//        Player = GameObject.FindGameObjectWithTag("Player");
//        if (Player != null)
//        {
//            PathFinder = GetComponent<PathFinder>();
//            //PathToTarget = PathFinder.GetPath(Player.transform.position);
//            //GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().floorForSpawnMob;
//            isMooving = true;


//            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2,2);
//            var position = transform.position;

//            position.x = p1p2[0];
//            position.y = p1p2[1];
//            movePos1.transform.position = position;
//            print($"{position.x},{position.y}");

//            Array.Copy(GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().GetPos1Pos2(), p1p2, 2);
//            position.x = p1p2[0];
//            position.y = p1p2[1];
//            movePos2.transform.position = position;
//            print($"{position.x},{position.y}");
//        }

//        damage = 3 * LevelGenerator.LVL;
//        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
//        health = 4.5f * LevelGenerator.LVL;
//        speed = UnityEngine.Random.Range(1f, 3f);
//    }

//    void Update()
//    {

//        Player = GameObject.FindGameObjectWithTag("Player");

//        if (Player == null) return;

//        if (Vector2.Distance(transform.position, Player.transform.position) < 3f)
//        {
//            timeToMove = 3f;
//            Move(Player, true);
//        }
//        if (Vector2.Distance(transform.position, Player.transform.position) >= 3f)
//        {
//            if (timeToMove <= 0)
//            {
//                if (go1)
//                {
//                    Move(movePos1, false);
//                    print(Vector2.Distance(transform.position, movePos1.transform.position));
//                    if (Vector2.Distance(transform.position, movePos1.transform.position) <= 1.5f)
//                    {
//                        go1 = false;
//                        timeToMove = 3f;
//                        print(go1);
//                    }
//                }
//                else
//                {
//                    Move(movePos2, false);
//                    print(Vector2.Distance(transform.position, movePos2.transform.position));
//                    if (Vector2.Distance(transform.position, movePos2.transform.position) <= 1.5f)
//                    {
//                        go1 = true;
//                        timeToMove = 3f;
//                        print(go1);
//                    }
//                }
//            }
//            else
//            {
//                timeToMove -= Time.deltaTime;
//            }


//        }
//    }
//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(attackPos.position, attackRange);
//    }

//    public void TakeDamage(float damage)
//    {
//        health -= damage;
//        if (health <= 0)
//        {
//            AmuletBuff.countDeadMobs++;
//            float r = (int)UnityEngine.Random.Range(0f, 4f);

//            if (r == 0)
//            {
//                Instantiate(HealthPotion, transform.position, Quaternion.identity);
//            }

//            else if (r == 1)
//            {
//                Instantiate(Scroll, transform.position, Quaternion.identity);
//            }

//            else if (r == 2)
//            {
//                Instantiate(Soull, transform.position, Quaternion.identity);
//            }

//            else if (r == 3)
//            {
//                Instantiate(HealthPotion, transform.position, Quaternion.identity);
//            }

//            r = UnityEngine.Random.Range(0f, 1f);
//            if (r <= DropAmuletChance(3, AmuletBuff.GdropCount, AmuletBuff.countDeadMobs))
//            {
//                Instantiate(GAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.GdropCount++;
//            }
//            if (r <= DropAmuletChance(2, AmuletBuff.BdropCount, AmuletBuff.countDeadMobs))
//            {
//                Instantiate(BAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.BdropCount++;
//            }
//            if (r <= DropAmuletChance(2, AmuletBuff.YdropCount, AmuletBuff.countDeadMobs))
//            {
//                Instantiate(YAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.YdropCount++;
//            }




//            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
//            Destroy(gameObject);
//        }
//    }

//    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
//    {
//        float res = ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
//        return res;
//    }

//    private void Move(GameObject target, bool player)
//    {
//        if (player)
//        {
//            if (Vector2.Distance(transform.position, Player.transform.position) > 0.7f)
//            {
//                PathToTarget = PathFinder.GetPath(Player);

//                isMooving = true;
//            }
//            if (PathToTarget.Count == 0)
//            {

//                Collider2D PlayerToDamage = Physics2D.OverlapCircle(attackPos.position, attackRange, whatIsEnemies);
//                if (PlayerToDamage != null)
//                {

//                    if (timeBtwAttac <= 0)
//                    {
//                        playerHP.TakingDamage(damage);
//                        timeBtwAttac = startTimeBtwAttac;
//                    }
//                    else
//                    {
//                        timeBtwAttac -= Time.deltaTime;
//                    }
//                }
//                return;
//            }


//            if (isMooving)
//            {
//                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
//                {
//                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);

//                }
//                else
//                {
//                    isMooving = false;
//                }
//            }
//            else
//            {
//                PathToTarget = PathFinder.GetPath(Player);
//                isMooving = true;
//            }
//        }


//        else
//        {
//            if (Vector2.Distance(transform.position, target.transform.position) > 0.2f)
//            {
//                PathToTarget = PathFinder.GetPath(target);

//                isMooving = true;
//            }
//            if (PathToTarget.Count == 0)
//            {                
//                return;
//            }


//            if (isMooving)
//            {
//                if (Vector2.Distance(transform.position, PathToTarget[PathToTarget.Count - 1]) > 0.1f)
//                {
//                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToTarget[PathToTarget.Count - 1].x, PathToTarget[PathToTarget.Count - 1].y, -91), speed * Time.deltaTime);

//                }
//                else
//                {
//                    isMooving = false;
//                }
//            }
//            else
//            {
//                PathToTarget = PathFinder.GetPath(target);
//                isMooving = true;
//            }
//        }
//    }

//}













//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy : MonoBehaviour
//{
//    public float health;

//    //путь
//    private List<Vector2> PathToPlayer = new List<Vector2>();
//    private PathFinder PathFinder;
//    private bool isMooving;
//    private GameObject Player;
//    public float speed;


//    //атака
//    private float damage;
//    private PlayerHP playerHP;
//    public Transform attackPos;
//    public float attackRange;
//    public float startTimeBtwAttac;
//    private float timeBtwAttac = 0;
//    public LayerMask whatIsEnemies;
//    public GameObject HealthPotion, Scroll, Soull, GAmulet, BAmulet, YAmulet;
//    void Start()
//    {
//        Player = GameObject.FindGameObjectWithTag("Player");
//        if (Player != null)
//        {
//            PathFinder = GetComponent<PathFinder>();
//            PathToPlayer = PathFinder.GetPath(Player.transform.position);
//            isMooving = true;
//        }
//        damage = 3 * LevelGenerator.LVL;
//        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
//        health = 4.5f * LevelGenerator.LVL;
//        speed = Random.Range(1f, 3f);
//    }


//    void Update()
//    {


//        if (Player == null) return;

//        if (Vector2.Distance(transform.position, Player.transform.position) > 0.7f)
//        {
//            PathToPlayer = PathFinder.GetPath(Player.transform.position);

//            isMooving = true;
//        }
//        if (PathToPlayer.Count == 0)
//        {

//            Collider2D PlayerToDamage = Physics2D.OverlapCircle(attackPos.position, attackRange, whatIsEnemies);
//            if (PlayerToDamage != null)
//            {

//                if (timeBtwAttac <= 0)
//                {
//                    playerHP.TakingDamage(damage);
//                    timeBtwAttac = startTimeBtwAttac;
//                }
//                else
//                {
//                    timeBtwAttac -= Time.deltaTime;
//                }
//            }
//            return;
//        }


//        if (isMooving)
//        {
//            if (Vector2.Distance(transform.position, PathToPlayer[PathToPlayer.Count - 1]) > 0.1f)
//            {
//                transform.position = Vector3.MoveTowards(transform.position, new Vector3(PathToPlayer[PathToPlayer.Count - 1].x, PathToPlayer[PathToPlayer.Count - 1].y, -91), speed * Time.deltaTime);

//            }
//            else
//            {
//                isMooving = false;
//            }
//        }
//        else
//        {
//            Player = GameObject.FindGameObjectWithTag("Player");
//            PathToPlayer = PathFinder.GetPath(Player.transform.position);
//            isMooving = true;
//        }
//    }
//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(attackPos.position, attackRange);
//    }

//    public void TakeDamage(float damage)
//    {
//        health -= damage;
//        print("Fay-Fay");
//        if (health <= 0)
//        {
//            AmuletBuff.countDeadMobs++;
//            Vector3 itemDropPos;
//            float r = (int)Random.Range(0f, 4f);

//            if (r == 0)
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(HealthPotion, transform.position, Quaternion.identity);
//            }

//            else if (r == 1)
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(Scroll, transform.position, Quaternion.identity);
//            }

//            else if (r == 2)
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(Soull, transform.position, Quaternion.identity);
//            }

//            else if (r == 3)
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(HealthPotion, transform.position, Quaternion.identity);
//            }

//            r = Random.Range(0f, 1f);
//            if (r <= DropAmuletChance(3, AmuletBuff.GdropCount, AmuletBuff.countDeadMobs))
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(GAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.GdropCount++;
//                print("я дропнулся");
//            }
//            if (r <= DropAmuletChance(2, AmuletBuff.BdropCount, AmuletBuff.countDeadMobs))
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(BAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.BdropCount++;
//                print("я дропнулся");
//            }
//            if (r <= DropAmuletChance(2, AmuletBuff.YdropCount, AmuletBuff.countDeadMobs))
//            {
//                //itemDropPos = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), -87);
//                Instantiate(YAmulet, transform.position, Quaternion.identity);
//                AmuletBuff.YdropCount++;
//                print("я дропнулся");
//            }




//            GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<LevelGenerator>().DecreaseMobCountOnLvl();
//            Destroy(gameObject);
//        }
//    }

//    float DropAmuletChance(float k, float dropCount, float countDeadMobs)
//    {
//        float res = ((k - dropCount) / (100 - countDeadMobs)) * 1.4f * (k - dropCount);
//        print(res);
//        return res;
//    }

//    private void Move(GameObject target)
//    {

//    }
//}
