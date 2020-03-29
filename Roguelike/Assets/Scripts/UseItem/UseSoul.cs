using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseSoul : MonoBehaviour
{
    public void Use()
    {
        int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>().UseSoul(selectedSlot);

    }
    public void Drop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
            gameObject.GetComponentInParent<Slot>().DropItem(selectedSlot);
        }
        else
        {
            Use();
        }
    }
}
