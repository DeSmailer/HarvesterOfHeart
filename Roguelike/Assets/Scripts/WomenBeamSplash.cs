using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WomenBeamSplash : MonoBehaviour
{
    private PlayerHP playerHP;
    public float damage;
    public float timeToDestroy;
    float timeBtwDamage;
    public AudioClip[] clips;
    AudioSource audioSource;
    void Start()
    {
        damage = 5 * LevelGenerator.LVL;
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        timeBtwDamage = timeToDestroy;

        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clips[0];
        print(audioSource.clip);
        audioSource.Play();
    }
    //переделывать под анимацию
    void Update()
    {
        var scale = transform.localScale;
        if (scale.x <= 1f)
        {
            scale.x += 0.01f;
            scale.y += 0.01f;
            transform.localScale = scale;
        }
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        playerHP.TakingDamage((damage * Time.deltaTime * 0.58f)/ timeBtwDamage);
    }
}
