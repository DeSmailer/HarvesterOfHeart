using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public bool[] isFull;

    public GameObject[] slots;

    public int selectedSlot;

    private bool inActive;

    public Sprite[] sprites = new Sprite[3];

    public Image imageInfo;
    public Text itemTextInfo;

    private void Start()
    {
        inActive = true;
        selectedSlot = 0;
        //GetTextInfo(selectedSlot);
    }

    private void Update()
    {
        if (Input.GetKeyUp("i"))
        {
            if (inActive == true)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i].gameObject.SetActive(false);
                }
                inActive = false;
            }
            else if (inActive == false)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i].gameObject.SetActive(true);

                }
                inActive = true;
            }
        }
        if (Input.GetKeyUp("left"))
        {
            slots[selectedSlot].GetComponent<Image>().sprite = sprites[0];
            selectedSlot--;
            if (selectedSlot < 0)
                selectedSlot = 7;
            if (selectedSlot != 7)
            {
                slots[7].GetComponent<Image>().sprite = sprites[2];
                slots[selectedSlot].GetComponent<Image>().sprite = sprites[1];
            }
                
            else
            slots[selectedSlot].GetComponent<Image>().sprite = sprites[1];
            //отображение инфы
            //GetTextInfo(selectedSlot);
        }
        if (Input.GetKeyUp("right"))
        {
            slots[selectedSlot].GetComponent<Image>().sprite = sprites[0];
            selectedSlot++;
            if (selectedSlot > 7)
                selectedSlot = 0;
            if (selectedSlot != 7)
            {
                slots[7].GetComponent<Image>().sprite = sprites[2];
                slots[selectedSlot].GetComponent<Image>().sprite = sprites[1];
            }

            else
                slots[selectedSlot].GetComponent<Image>().sprite = sprites[1];
            //отображение инфы
            //GetTextInfo(selectedSlot);
        }
        if (Input.GetKeyUp("up"))
        {
            Up(selectedSlot);
        }
        //одеть\cнять итем
        if (Input.GetKeyUp("down"))
        {
            Down(selectedSlot);
        }
    }
    //public void GetTextInfo(int selectedSlot)
    //{
    //    var position = imageInfo.transform.position;
    //    position.x = (slots[selectedSlot].transform.position.x + 95);
    //    imageInfo.transform.position = position;
    //    itemTextInfo.text = slots[selectedSlot].GetComponent<Slot>().GetInfo();
    //}
    public void Up(int selectedSlot)
    {
        if (inActive == true)
        {
            slots[selectedSlot].GetComponent<Slot>().DropItem(selectedSlot);
            //GetTextInfo(selectedSlot);
        }
    }
    public void Down(int selectedSlot)
    {
        if (inActive == true)
        {
            if (selectedSlot == 7)
            {
                if (isFull[7])
                    slots[selectedSlot].GetComponent<Slot>().PutOutItem(7);
            }
            else
            {
                slots[selectedSlot].GetComponent<Slot>().PutOnItem(selectedSlot);
            }
        }
    }
}
