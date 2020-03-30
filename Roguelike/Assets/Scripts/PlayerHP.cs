using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public float maxHP;//максимальное хп без шмотки
    public float currentMaxHP;//максимальное хп под бафом шмотки
    public float currentHP;//текущее

    public float maxDamageRatio;//максимальный коеф без шмотки    
    public float currentMaxDamageRatio;//текущий под бафом шмотки
    public float currentDamageRatio;//текущий коеф дамага


    public Slider slider;
    public Image fillImage;
    public float timeForScroll;


    private Inventory inventory;
    private Color color1;
    private Color color2;
    private int changeColorTime = 5;

    public bool attackable;

    public GameObject Center;
    private Animator CenterAnim;
    private SpriteRenderer spriteRenderer;
    private bool isRed;
    private float redVariable;
    public AudioClip[] clips;
    AudioSource audioSource;
    private bool isPlayedHP = false;
    private bool isPlayedScroll = false;

    void Start()
    {
        maxHP = 12f;
        currentHP = currentMaxHP = maxHP;
        maxDamageRatio = currentMaxDamageRatio = 1;
        DisplayHP();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        color1 = new Color(255, 255, 255, 1f);
        color2 = new Color(255, 255, 255, 0f);
        fillImage.color = color1;
        attackable = true;
        CenterAnim = Center.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRed = false;
        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (currentHP / currentMaxHP < 0.15f)
        {
            if (changeColorTime <= 0)
            {
                if (fillImage.color == color1)
                {
                    fillImage.color = color2;
                }
                else if (fillImage.color == color2)
                {
                    fillImage.color = color1;
                }
                changeColorTime = 10;
            }
            else
            {
                changeColorTime--;
            }
        }
        if (Input.GetKeyUp("1"))
        {
            UseHPPotion();
        }
        if (Input.GetKeyUp("3"))
        {
            UseScroll();
        }
        ChangeColor();
    }

    private void FixedUpdate()
    {
        if (timeForScroll > 0f)
        {
            timeForScroll -= Time.deltaTime;
            CenterAnim.SetInteger("state", 3);
            if (!isPlayedScroll)
            {
                audioSource.clip = clips[1];
                print(audioSource.clip);
                audioSource.Play();
                isPlayedScroll = true;
            }
        }
        else
        {
            currentDamageRatio = currentMaxDamageRatio;
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
                CenterAnim.SetInteger("state", 0);
        }
        if (!isPlayedHP)
        {
            audioSource.clip = clips[0];
            print(audioSource.clip);
            audioSource.Play();
            isPlayedHP = true;
        }
    }
    //получение урона и снижение его под бафами свитка
    public void TakingDamage(float damage)
    {
        print(attackable);
        if (attackable)
        {
            currentHP = currentHP - damage * currentDamageRatio;
            DisplayHP();
            isRed = true;
            print(isRed);
            redVariable = 1.5f;
            print(redVariable);
            if (currentHP <= 0)
            {
                SceneManager.LoadScene("OnLoseScene");
            }
        }
    }

    //восполнение хп
    public void UseHPPotion()
    {
        for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
        {
            if (inventory.slots[i].transform.childCount > 0)
            {
                if (inventory.slots[i].transform.GetChild(0).CompareTag("HealthPotion"))
                {
                    currentHP += maxHP * 0.05f;
                    CenterAnim.SetInteger("state", 1);
                    if (currentHP > currentMaxHP)
                    {
                        currentHP = currentMaxHP;
                    }
                    isPlayedHP = false;
                    DisplayHP();
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
        DisplayHP();
    }
    public void UseHPPotion(int selSlot)
    {
        if (inventory.slots[selSlot].transform.childCount > 0)
        {
            if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("HealthPotion"))
            {
                currentHP += maxHP * 0.05f;
                CenterAnim.SetInteger("state", 1);
                if (currentHP > currentMaxHP)
                {
                    currentHP = currentMaxHP;
                }
                isPlayedHP = false;
                DisplayHP();
                inventory.isFull[selSlot] = false;
                foreach (Transform t in inventory.slots[selSlot].transform)
                {
                    Destroy(t.gameObject);
                }
            }
        }
        DisplayHP();
    }
    //public void UseHPPotionOnButton()
    //{
    //    GameObject parent = transform.parent.gameObject;
    //    if (parent.CompareTag("Slot"))
    //    {
    //        int n = parent.GetComponent<Slot>().number;
    //        UseHPPotion();
    //        inventory.isFull[n] = false;
    //        foreach (Transform t in inventory.slots[n].transform)
    //        {
    //            Destroy(t.gameObject);
    //        }
    //    }


    //    DisplayHP();
    //}

    public void UseScroll()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
        {
            for (int i = 0; i < inventory.slots.Length; i++) //inventory.slots.Length
            {
                if (inventory.slots[i].transform.childCount > 0)
                {
                    if (inventory.slots[i].transform.GetChild(0).CompareTag("Scroll"))
                    {
                        currentDamageRatio = currentMaxDamageRatio * 0.75f;
                        timeForScroll = 15f;
                        CenterAnim.SetInteger("state", 3);
                        inventory.isFull[i] = false;
                        isPlayedScroll = false;
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

    public void UseScroll(int selSlot)
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().timeForSoul <= 0 && timeForScroll <= 0)
        {
            if (inventory.slots[selSlot].transform.childCount > 0)
            {
                if (inventory.slots[selSlot].transform.GetChild(0).CompareTag("Scroll"))
                {
                    currentDamageRatio = currentMaxDamageRatio * 0.75f;
                    timeForScroll = 15f;
                    CenterAnim.SetInteger("state", 3);
                    isPlayedScroll = false;
                    inventory.isFull[selSlot] = false;
                    foreach (Transform t in inventory.slots[selSlot].transform)
                    {
                        Destroy(t.gameObject);
                    }
                }
            }
        }
    }


    private void DisplayHP()
    {
        float HPSlider = currentHP / currentMaxHP;

        if (HPSlider < 100)
        {
            if (HPSlider > 0.51f && HPSlider < 0.90f)
            {
                slider.value = HPSlider * 0.7f;
            }
            else if (HPSlider > 0.11f && HPSlider < 0.50f)
            {
                slider.value = HPSlider * 0.6f;
            }
            else if (HPSlider > 0.2f && HPSlider < 0.10f)
            {
                slider.value = HPSlider * 0.5f;
            }
            else
            {
                slider.value = HPSlider;
            }
        }
        else
        {
            slider.value = HPSlider;
        }

        //if(HPSlider > 0.10f)
        //{
        //    //fillImage.color = color1;
        //    slider.value = HPSlider;
        //}

        //else if (HPSlider <= 0.10f)
        //{
        //    if (HPSlider < 0.05f)
        //    {
        //        slider.value = HPSlider;
        //    }
        //    else slider.value = 0.05f;
        //}


    }

    public void LvlHPUp()
    {
        bool Is = false;
        if (currentMaxHP > maxHP)
            Is = true;
        maxHP = 16 * LevelGenerator.LVL;
        //тут еще что то нужно, типо когда на некст лвл переход, и есть активная шмотка, то и карентМакс нужно увеличить
        //пошаманить потом
        currentMaxHP = maxHP;
        if (Is)
        {
            AmuletBuff.SetBuff(0.1f, 0, 1f);
        }
        currentHP += 12f;
        if (currentHP > currentMaxHP)
        {
            currentHP = currentMaxHP;
        }
        DisplayHP();
    }

    public void HPBuff(float hpBuff)
    {
        currentMaxHP = maxHP * (1 + hpBuff);
        DisplayHP();
    }
    public void RatioBuff(float ratioBuff)
    {
        currentMaxDamageRatio = maxDamageRatio * ratioBuff;
        if (currentHP > currentMaxHP)
        {
            currentHP = currentMaxHP;
        }
        DisplayHP();
    }
    public void SetNonBuffAnimation()
    {
        CenterAnim.SetInteger("state", 0);
    }
   private void ChangeColor()
    {
        if (isRed)
        {
            if (redVariable >=1)
            {
                redVariable -= 0.03f;
                spriteRenderer.color = new Color(1f, 1f / redVariable,1f / redVariable, 1f);
            }
            else
            {
                isRed = false;
            }
                
        }
    }
}

