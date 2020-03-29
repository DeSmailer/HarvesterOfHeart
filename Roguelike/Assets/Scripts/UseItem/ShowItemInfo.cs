using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowItemInfo : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[3];

    public GameObject imageInfo;
    public Text itemTextInfo;
    private Inventory inventory;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        itemTextInfo = imageInfo.transform.GetChild(0).GetComponent<Text>();
    }
    public void GetTextInfo()
    {
        ShowImage();
        DestroyTextInfo();
        ShowImage();
    }
    public void DestroyTextInfo()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }
    private void ShowImage()
    {
        int selectedSlot = gameObject.GetComponentInParent<Slot>().number;
        var position = imageInfo.transform.position;
        position.x = (inventory.slots[selectedSlot].transform.position.x + 40);
        position.y = (inventory.slots[selectedSlot].transform.position.y + 100);
        itemTextInfo.text = inventory.slots[selectedSlot].GetComponent<Slot>().GetInfo();
        Instantiate(imageInfo, transform, true);
        imageInfo.transform.position = position;
    }
}
