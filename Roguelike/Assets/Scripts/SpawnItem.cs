using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public GameObject item;
    private Transform player;
    //public int slotNumber;
    public string type;
    public AudioClip[] clips;
    AudioSource audioSource;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[0];
        print(audioSource.clip);
        audioSource.Play();
    }
    public void SpawnDroppedItem()
    {
        Vector3 playerPos = new Vector3(player.position.x, player.position.y - 0.15f, -87);
        Instantiate(item, playerPos, Quaternion.identity);
    }
    //public void SetSlotNumber(int i)
    //{
    //    slotNumber = i;
    //}
}
