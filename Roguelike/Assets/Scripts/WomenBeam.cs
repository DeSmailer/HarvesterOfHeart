using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomenBeam : MonoBehaviour
{
    public float damage;
    public float timeBtwAttac;
    private PlayerHP playerHP;
    public GameObject attackPos;
    public float attackRange;
    public LayerMask whatIsEnemies;
    // Start is called before the first frame update
    void Start()
    {
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        damage = 5 * LevelGenerator.LVL;
    }

    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.transform.position, attackRange, whatIsEnemies);
        if (enemiesToDamage.Length > 0)
        {
            playerHP.TakingDamage(damage / 16);
        }   
            
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.transform.position, attackRange);
    }

    public void RIP()
    {
        Destroy(gameObject);
    }
}
