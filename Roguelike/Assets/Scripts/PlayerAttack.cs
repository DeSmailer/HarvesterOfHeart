using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;


    public float startTimeBtwAttac;
    public float timeForScroll;

    public float MaxDamage; //максимальный урон без шмотки
    public float currentMaxDamage; //максимальный урон под бафом шмотки
    public float currentDamage;//текущее

    private Inventory inventory;
    private float timeBtwAttac = 0;
    private float timeBtwAttacForAttack2 = 0;
    int attackCount = 0;

    public Animator anim;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        currentMaxDamage = 2;
        currentDamage = MaxDamage = currentMaxDamage;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyUp("3"))
        {
            if (timeForScroll <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForSoul <= 0)
                UseScroll();
        }

        if (timeBtwAttac <= 0)
        {
            if (Input.GetKeyUp("space"))
            {
                if (attackCount == 1 && timeBtwAttacForAttack2 <= startTimeBtwAttac + 1)
                {
                    bool r = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().attackPositiontR;

                    if (r)
                    {
                        anim.SetInteger("state", 5);
                    }
                    else
                    {
                        anim.SetInteger("state", 6);
                    }
                }

                else
                {
                    bool r = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().attackPositiontR;

                    if (r)
                    {
                        anim.SetInteger("state", 3);
                    }
                    else
                    {
                        anim.SetInteger("state", 4);
                    }
                }
            }
            else
            {
                timeBtwAttacForAttack2 += Time.deltaTime;
            }
        }
        else
        {
            timeBtwAttac -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (timeForScroll > 0f)
        {
            timeForScroll -= Time.deltaTime;
        }
        else
        {
            currentDamage = currentMaxDamage;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public void UseScroll()
    {
        for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
        {
            if (inventory.slots[i].transform.childCount > 0)
            {
                if (inventory.slots[i].transform.GetChild(0).CompareTag("Scroll"))
                {
                    currentDamage = currentMaxDamage * 1.25f;
                    timeForScroll = 15f;
                    inventory.isFull[i] = false;
                    foreach (Transform t in inventory.slots[i].transform)
                    {
                        Destroy(t.gameObject);
                    }
                    break;
                }
            }
            else
            {
                continue;
            }

        }
    }
    public void DamageBuff(float damaheBuff)
    {
        currentMaxDamage = MaxDamage * (1 + damaheBuff);
    }
    public void LvlDamageUp()
    {
        {
            bool Is = false;
            if (currentMaxDamage > MaxDamage)
                Is = true;
            MaxDamage = 2 * LevelGenerator.LVL;
            currentMaxDamage = MaxDamage;
            //тут еще что то нужно, типо когда на некст лвл переход, и есть активная шмотка, то и карентМакс нужно увеличить
            if (Is)
            {
                AmuletBuff.SetBuff(0, 0.1f, 1);
            }
        }
    }

    public void Attack1()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {

            if (enemiesToDamage[i].GetComponent<Enemy>() != null)
                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(currentDamage);
            if (enemiesToDamage[i].GetComponent<Boss1Enemy>() != null)
                enemiesToDamage[i].GetComponent<Boss1Enemy>().TakeDamage(currentDamage);
            if (enemiesToDamage[i].GetComponent<Boss2Enemy>() != null)
                enemiesToDamage[i].GetComponent<Boss2Enemy>().TakeDamage(currentDamage);

        }
    }

    public void SetStartTimeBtwAttac()
    {
        timeBtwAttac = startTimeBtwAttac;
    }


    public void CheckStartTimeBtwAttac2()
    {
        if (timeBtwAttacForAttack2 <= startTimeBtwAttac + 1)
        {
            attackCount++;
            timeBtwAttacForAttack2 = 0;
        }
        else
        {
            attackCount = 0;
            timeBtwAttacForAttack2 = 0;
        }
    }
    public void SetStartTimeBtwAttac2()
    {
        attackCount = 0;
        timeBtwAttacForAttack2 = 0;
    }
}
