using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Inventory inventory;

    public GameObject itemButton;

    public string type;

    public AudioClip[] clips;
    AudioSource audioSource;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        StartCoroutine(Drop());
        audioSource = GetComponent<AudioSource>();
    }


    public void OnTriggerStay2D(Collider2D other) //PickUpToSlot OnTriggerEnter2D
    {
        if (Input.GetKeyUp("e"))
        {
            if (other.CompareTag("Player"))
            {
                audioSource.clip = clips[0];
                print(audioSource.clip);
                audioSource.Play();
                if (type == "consumable")
                {
                    for (int i = 0; i < inventory.slots.Length - 1; i++)
                    {
                        if (inventory.isFull[i] == false)
                        {
                            //добавляем
                            inventory.isFull[i] = true;
                            Instantiate(itemButton, inventory.slots[i].transform, false);
                            //if (inventory.selectedSlot == i)
                            //inventory.GetTextInfo(i);
                            //inventory.slots[i].transform.GetChild(0).GetComponent<SpawnItem>().SetSlotNumber(i);
                            //print(inventory.slots[i].transform.GetChild(0).GetComponent<SpawnItem>());
                            //print(inventory.slots[i].transform.GetChild(0).GetComponent<SpawnItem>().slotNumber);
                            Destroy(gameObject);
                            break;
                        }
                    }
                }
                else if (type == "amulet")
                {
                    if (inventory.isFull[7] == false)
                    {
                        inventory.isFull[7] = true;
                        Instantiate(itemButton, inventory.slots[7].transform, false);
                        if (inventory.selectedSlot == 7)
                            //inventory.GetTextInfo(7);
                            inventory.slots[7].GetComponent<Slot>().PutOnItem(7);
                        foreach (Transform child in inventory.slots[7].transform)
                        {
                            if (child.CompareTag("BAmulet"))
                            {
                                AmuletBuff.SetBuff(0, 0.1f, 1);
                            }
                            if (child.CompareTag("GAmulet"))
                            {
                                AmuletBuff.SetBuff(0.1f, 0, 1);
                            }
                            if (child.CompareTag("YAmulet"))
                            {
                                AmuletBuff.SetBuff(0, 0, 0.91f);
                            }
                            //inventory.slots[7].transform.GetChild(0).GetComponent<SpawnItem>().slotNumber = 7;
                            Destroy(gameObject);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < inventory.slots.Length - 1; i++)
                        {
                            if (inventory.isFull[i] == false)
                            {
                                //добавляем
                                inventory.isFull[i] = true;
                                Instantiate(itemButton, inventory.slots[i].transform, false);
                                //if (inventory.selectedSlot == i)
                                    //inventory.GetTextInfo(i);
                                    //inventory.slots[i].transform.GetChild(0).GetComponent<SpawnItem>().slotNumber = i;
                                Destroy(gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    //прикольно но странно
    IEnumerator Drop()
    {
        var position = transform.position;
        for (float n = 2.9f; n <= 3.2f; n += 0.005f)
        {
            if (n >= 3)
            {
                position.x = transform.position.x + 0.01f;
                position.y = transform.position.y - (-Mathf.Pow((n - 3), 2) + 9)/ 500;
                //print($"{n}, {(-Mathf.Pow((n - 3), 2) + 9)}");
                transform.position = position;
            }
            else
            {
                position.y = transform.position.y + 0.05f;
                transform.position = position;
            }

            yield return null;
        }

    }
}

    
