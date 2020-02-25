using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private enum State
    {
        Normal,
        Rolling,
    }

    private Rigidbody2D rb2d;

    public Animator anim;

    float goHorizontal;
    float goVertical;

    private Vector3 moveDir;
    private Vector3 rollDir;
    public float rollSpeed;
    private float distanceTraveled;

    private State state;
    //Используем это для инициализации
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        speed = 120f;
        anim = GetComponent<Animator>();
        state = State.Normal;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Normal:
                goHorizontal = Input.GetAxisRaw("Horizontal");
                goVertical = Input.GetAxisRaw("Vertical");


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rollDir = moveDir;
                    rollSpeed = 21f;
                    state = State.Rolling;
                    distanceTraveled = 0;
                }
                break;

            case State.Rolling:
                float rollSpeedDropMultiplier = 0.05f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = 20f;
                if (rollSpeed <= rollSpeedMinimum)
                {
                    state = State.Normal;
                }
                break;
        }
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                if (goHorizontal < 0)
                {
                    anim.SetInteger("state", 1);
                }
                else if (goHorizontal > 0)
                {
                    anim.SetInteger("state", 2);
                }
                else if (goHorizontal == 0)
                {
                    anim.SetInteger("state", 0);
                }
                if (goHorizontal == 0 && goVertical == 1)
                {
                    anim.SetInteger("state", 7);
                }
                else if (goHorizontal == 0 && goVertical == -1)
                {
                    anim.SetInteger("state", 8);
                }

                moveDir = new Vector3(goHorizontal, goVertical).normalized;
                rb2d.velocity = moveDir * speed * Time.deltaTime;
                break;


            case State.Rolling:
                rb2d.velocity = rollDir * rollSpeed ;
                distanceTraveled += rollSpeed * Time.deltaTime;

                if (distanceTraveled >= 1.8)
                {
                    print(distanceTraveled);
                    state = State.Normal;
                }
                    

                break;
        }
    }
}
   
