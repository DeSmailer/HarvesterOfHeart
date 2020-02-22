using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss1Eemy : MonoBehaviour
{
    public float currentHP;
    public float maxHP;

    //путь
    private List<Vector2> PathToPlayer = new List<Vector2>();
    private PathFinder PathFinder;
    private bool isMooving;

    public GameObject Player;

    public float speed;
    public Slider slider;
    public float offset;
    public GameObject womenBeam;


    //атака
    private float damage;
    private PlayerHP playerHP;
    public float attackRange;
    public float startTimeBtwAttac;
    private float timeBtwAttac = 0;
    public LayerMask whatIsEnemies;
    public GameObject HealthPotion, Scroll, Soull;
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
        maxHP = currentHP = 40 * LevelGenerator.LVL;
        speed = Random.Range(1f, 3f);
        DisplayHP();
    }


    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player == null) return;

        if (timeBtwAttac <= 0)
        {
            Shoot(Player.transform);
            timeBtwAttac = startTimeBtwAttac;
        }
        else
        {
            timeBtwAttac -= Time.deltaTime;
        }

        if (PathToPlayer.Count == 0 && Vector2.Distance(transform.position, Player.transform.position) > 2f) 
        {
            PathToPlayer = PathFinder.GetPath(Player.transform.position);

            isMooving = true;
        }
        if (PathToPlayer.Count == 0)
        {

            //Collider2D PlayerToDamage = Physics2D.OverlapCircle(attackPos.position, attackRange, whatIsEnemies);
            //if (PlayerToDamage != null)
            //{

            //    if (timeBtwAttac <= 0)
            //    {
            //        playerHP.TakingDamage(damage);
            //        timeBtwAttac = startTimeBtwAttac;
            //    }
            //    else
            //    {
            //        timeBtwAttac -= Time.deltaTime;
            //    }
            //}
            return;
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
            PathToPlayer = PathFinder.GetPath(Player.transform.position);
            isMooving = true;
        }


    }
    void Shoot(Transform attackPos)
    {
        Instantiate(womenBeam,new Vector3( attackPos.transform.position.x, attackPos.transform.position.y + offset, -95), Quaternion.identity);
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(attackPos.position, attackRange);
    //}
    private void DisplayHP()
    {
        float HPSlider = currentHP / maxHP;
        slider.value = HPSlider;
    }
}
