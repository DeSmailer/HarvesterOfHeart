using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    private Inventory inventory;

    public int number;

    public Transform temporarySlot;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    public void DropItem(int selectedSlot)
    {
        if (selectedSlot == 7)
        {
            if (inventory.isFull[7] == true)
            {
                AmuletBuff.SetBuff(0, 0, 1);
            }

        }
        foreach (Transform child in transform)
        {
            child.GetComponent<SpawnItem>().SpawnDroppedItem();
            GameObject.Destroy(child.gameObject);
            inventory.isFull[selectedSlot] = false;
        }
        //inventory.GetTextInfo(selectedSlot);
    }
    public string GetInfo()
    {
        string info = "NONE";
        foreach (Transform child in transform)
        {
            info = child.GetComponent<ItemInfo>().ReturnString();
        }
        return info;
    }
    //предмет из 0-6 в 7
    public void PutOnItem(int selectedSlot)
    {
        string type = "";
        foreach (Transform child in transform)
        {
            type = child.GetComponent<SpawnItem>().type;
        }
        if (type == "amulet")
        {
            if (inventory.isFull[7] == false)
            {
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("BAmulet"))
                    {
                        Instantiate(child, inventory.slots[7].transform, false);
                        AmuletBuff.SetBuff(0, 0.1f, 1);
                        inventory.isFull[selectedSlot] = false;
                        inventory.isFull[7] = true;
                    }
                    if (child.CompareTag("GAmulet"))
                    {
                        Instantiate(child, inventory.slots[7].transform, false);
                        AmuletBuff.SetBuff(0.1f, 0, 1);
                        inventory.isFull[selectedSlot] = false;
                        inventory.isFull[7] = true;
                    }
                    if (child.CompareTag("YAmulet"))
                    {
                        Instantiate(child, inventory.slots[7].transform, false);
                        AmuletBuff.SetBuff(0, 0, 0.91f);
                        inventory.isFull[selectedSlot] = false;
                        inventory.isFull[7] = true;
                    }
                }
                foreach (Transform child in transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            else
            {
                SwapItem(selectedSlot);
            }
        }
    }

    public void PutOutItem(int selectedSlot)
    {
        for (int i = 0; i < inventory.slots.Length - 1; i++)
        {
            if (inventory.isFull[i] == false)
            {
                //добавляем
                inventory.isFull[i] = true;
                foreach (Transform child in transform)
                {
                    Instantiate(child, inventory.slots[i].transform, false);
                    GameObject.Destroy(child.gameObject);
                }
                inventory.isFull[selectedSlot] = false;
                AmuletBuff.SetBuff(0, 0, 1);

                break;
            }
        }
    }

    public void SwapItem(int selectedSlot)
    {
        if (selectedSlot != 7)
        {

            foreach (Transform child in inventory.slots[7].transform)
            {
                Instantiate(child, temporarySlot, false);
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in transform)
            {
                if (child.CompareTag("BAmulet"))
                {
                    Instantiate(child, inventory.slots[7].transform, false);
                    GameObject.Destroy(child.gameObject);
                    AmuletBuff.SetBuff(0, 0.1f, 1);
                }
                if (child.CompareTag("GAmulet"))
                {
                    Instantiate(child, inventory.slots[7].transform, false);
                    GameObject.Destroy(child.gameObject);
                    AmuletBuff.SetBuff(0.1f, 0, 1);
                }
                if (child.CompareTag("YAmulet"))
                {
                    Instantiate(child, inventory.slots[7].transform, false);
                    GameObject.Destroy(child.gameObject);
                    AmuletBuff.SetBuff(0, 0, 0.91f);
                }
            }



            foreach (Transform child in temporarySlot)
            {
                if (child.CompareTag("BAmulet"))
                {
                    Instantiate(child, transform, false);
                    GameObject.Destroy(child.gameObject);
                }
                if (child.CompareTag("GAmulet"))
                {
                    Instantiate(child, transform, false);
                    GameObject.Destroy(child.gameObject);
                }
                if (child.CompareTag("YAmulet"))
                {
                    Instantiate(child, transform, false);
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }
}
