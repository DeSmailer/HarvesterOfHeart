using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Vector3 Player;

    public float damage;
    public float speed;
    private PlayerHP playerHP;
    private Vector3 V = new Vector3(0, 0, -90);
    public float smoothTime = 0.9F;
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform.position;
        damage = 5 * LevelGenerator.LVL;
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        print(Player);
    }


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player, speed * Time.deltaTime);
        if (transform.position == Player)
        {
            Destroy(gameObject);
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>().attackable)
            {
                playerHP.TakingDamage(damage);
                Destroy(gameObject);
            }
        }

        else if (other.gameObject.CompareTag("wall"))
            Destroy(gameObject);
    }
}
