using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LichoMovement : MonoBehaviour
{
    public float velocidadX;
    public float velocidady;
    public float fallingImprove;
    public float fallingAtClimb;

    private bool falling = false;
    private bool climb = false;
    public bool grounded =false;

    private Collider2D previous;

    private Rigidbody2D body;

    public KeyCode jump;
    public KeyCode fall;

    private int currentCollisions = 0;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }


    void Update()
    {

       body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * velocidadX, body.velocity.y);


        if (Input.GetKey(jump) && grounded) 
        {
            body.velocity = new Vector2(body.velocity.x, velocidady);
        }

        if (Input.GetKey(fall) && !grounded)
        {
            body.velocity = new Vector2(body.velocity.x, -fallingImprove);
        }


        if (climb)
        {
            body.velocity = new Vector2(body.velocity.x, -fallingAtClimb);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Suelo" || collision.collider.tag == "Escalable")
        {
            currentCollisions++;
            grounded = true;
            Debug.Log("Registered in");
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Suelo" || collision.collider.tag == "Escalable")
        {
            //grounded = false;
            Debug.Log("Registered out");
            currentCollisions--;

            if (currentCollisions == 0)
            {
                grounded = false;
            }
        }
       
    }
}
