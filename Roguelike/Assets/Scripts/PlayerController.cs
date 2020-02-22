using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;             

    private Rigidbody2D rb2d;

    public bool attackPositiontR;

    public Animator anim;

    float goHorizontal;
    float goVertical;

    //Используем это для инициализации
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        speed = 120f;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        goHorizontal = Input.GetAxisRaw("Horizontal");
        goVertical = Input.GetAxisRaw("Vertical");

        if(goHorizontal < 0)
        {
            SetPosAttal(false);
            anim.SetInteger("state", 1);
        }
        else if (goHorizontal > 0)
        {
            SetPosAttal(true);
            anim.SetInteger("state", 2);
        }
        else if(goHorizontal == 0)
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

        rb2d.velocity = new Vector2(goHorizontal, goVertical) * speed * Time.deltaTime;
    }
    public void SetPosAttal(bool r)
    {
        if (r)
            attackPositiontR = true;
        else
            attackPositiontR = false;
    }
}
