using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBAmulet : MonoBehaviour
{
    public void Use()
    {
        int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().Down(selectedSlot);
    }
    public void Drop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().Up(selectedSlot);
        }
        else
        {
            Use();
        }
    }
}        
    

