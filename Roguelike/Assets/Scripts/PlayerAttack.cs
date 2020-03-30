using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange;


    public float startTimeBtwAttac;
    public float timeForSoul;

    public float MaxDamage; //максимальный урон без шмотки
    public float currentMaxDamage; //максимальный урон под бафом шмотки
    public float currentDamage;//текущее

    private Inventory inventory;
    private float timeBtwAttac = 0;
    private float timeBtwAttacForAttack2 = 0;
    int attackCount = 0;
    public Animator anim;

    private Camera cam;

    Vector2 mousePoint;

    public GameObject Center;
    private Animator CenterAnim;
    private bool isPlayed = false;
    public AudioClip[] clips;
    AudioSource audioSource;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        currentMaxDamage = 2;
        currentDamage = MaxDamage = currentMaxDamage;
        anim = GetComponent<Animator>();
        cam = Camera.main;
        CenterAnim = Center.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (Input.GetKeyUp("2"))
        {
                UseSoul();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (timeBtwAttac <= 0)
            {
                byte r = CalculateAngle();

                switch (r)
                {
                    case 1:
                        anim.SetInteger("state", 4);
                        break;
                    case 2:
                        anim.SetInteger("state", 9);
                        break;
                    case 3:
                        anim.SetInteger("state", 3);
                        break;
                    case 4:
                        anim.SetInteger("state", 25);
                        break;
                    default:
                        anim.SetInteger("state", 3);
                        break;
                }
            }
            else
            {
                timeBtwAttac -= Time.deltaTime;
            }
        }
        else if (timeBtwAttac > 0)
        {
            timeBtwAttac -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (timeForSoul > 0f)
        {
            timeForSoul -= Time.deltaTime;
            CenterAnim.SetInteger("state", 2);
            if (!isPlayed)
            {
                audioSource.clip = clips[0];
                print(audioSource.clip);
                audioSource.Play();
                isPlayed = true;
            }
        }
        else
        {
            currentDamage = currentMaxDamage;
            if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
                CenterAnim.SetInteger("state", 0);

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    //public void UseScroll()
    //{
    //    for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
    //    {
    //        if (inventory.slots[i].transform.childCount > 0)
    //        {
    //            if (inventory.slots[i].transform.GetChild(0).CompareTag("Scroll"))
    //            {
    //                currentDamage = currentMaxDamage * 1.25f;
    //                timeForScroll = 15f;
    //                inventory.isFull[i] = false;
    //                foreach (Transform t in inventory.slots[i].transform)
    //                {
    //                    Destroy(t.gameObject);
    //                }
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            continue;
    //        }

    //    }
    //}
    public void UseSoul()
    {
        if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
            {
                if (inventory.slots[i].transform.childCount > 0)
                {
                    if (inventory.slots[i].transform.GetChild(0).CompareTag("Soul"))
                    {
                        currentDamage = currentMaxDamage * 1.25f;
                        timeForSoul = 15f;
                        CenterAnim.SetInteger("state", 2);
                        inventory.isFull[i] = false;
                        isPlayed = false;
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
    }
    public void UseSoul(int selSlot)
    {
        if (timeForSoul <= 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().timeForScroll <= 0)
        {
            if (inventory.slots[selSlot].transform.childCount > 0)
            {
                if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("Soul"))
                {
                    currentDamage = currentMaxDamage * 1.25f;
                    timeForSoul = 15f;
                    CenterAnim.SetInteger("state", 2);
                    inventory.isFull[selSlot] = false;
                    isPlayed = false;
                    foreach (Transform t in inventory.slots[selSlot].transform)
                    {
                        Destroy(t.gameObject);
                    }
                }
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
        audioSource.clip = clips[1];
        print(audioSource.clip);
        audioSource.Play();
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].GetComponent<Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<SmallEnemy>() != null)
            {
                enemiesToDamage[i].GetComponent<SmallEnemy>().TakeDamage(currentDamage);
            } 
            else if (enemiesToDamage[i].GetComponent<Boss1Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss1Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<Boss2Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss2Enemy>().TakeDamage(currentDamage);
            }
            else if (enemiesToDamage[i].GetComponent<Boss3Enemy>() != null)
            {
                enemiesToDamage[i].GetComponent<Boss3Enemy>().TakeDamage(currentDamage);
            }
        }
    }

    public void SetStartTimeBtwAttac()
    {
        timeBtwAttac = startTimeBtwAttac;
    }


    //public void CheckStartTimeBtwAttac2()
    //{
    //    if (timeBtwAttacForAttack2 <= startTimeBtwAttac + 1)
    //    {
    //        attackCount++;
    //        timeBtwAttacForAttack2 = 0;
    //    }
    //    else
    //    {
    //        attackCount = 0;
    //        timeBtwAttacForAttack2 = 0;
    //    }
    //}
    //public void SetStartTimeBtwAttac2()
    //{
    //    attackCount = 0;
    //    timeBtwAttacForAttack2 = 0;
    //}
    private byte CalculateAngle()
    {
        Vector2 playerPos = transform.position;

        Vector2 dir = playerPos - (Vector2)mousePoint;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle >= -45 && angle <= 45)
        {
            return 1;
        }
        else if (angle >= 45 && angle <= 135)
        {
            return 2;
        }
        else if (angle >= 135 || angle <= -135)
        {
            return 3;
        }
        else if (angle >= -135 && angle <= -45)
        {
            return 4;
        }
        else return 1;
    }
    //Screen.width / 2 - Input.mousePosition.x и Screen.height / 2 - Input.mousePosition.y


    void OnGUI()
    {
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        mousePoint = cam.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        //GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        //GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        //GUILayout.Label("Mouse position: " + mousePos);
        //GUILayout.Label("World position: " + mousePoint.ToString("F3"));
        //GUILayout.EndArea();
    }
}
