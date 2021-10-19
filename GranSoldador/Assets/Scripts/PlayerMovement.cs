using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    //Velocidades
    public float velocidadX;
    public float velocidady;
    private float currentVel;
    public float accX;
    
    public float fallingImprove;
    public float fallingAtClimb;
   
    public float dashVel;
    
    public float planeVelUp;
    public float planeVelDown;

    public float timeForDash, StartTimeDash, coolDownDash, currentTimeDash;
    public float timeForPlane, StartTimePlane;

    //RigidBody
    private Rigidbody2D body;
    //Contador de colisiones
    private int currentCollisions = 0;

    //Teclas
    public KeyCode jump;
    public KeyCode fall;
    public KeyCode special;

    //Flags de posicion
    private bool climb = false;
    public bool grounded = false;
    private bool lookingRight = true;

    //Habilidades
    public bool canClimb = false;
    public bool canRoll = false;
    public bool canDash = false;
    public bool canPlane = false;
    public bool canDoubleJump = false;
    public bool canHook = false;

    //Cosas del hook
    public LineRenderer line;
    public DistanceJoint2D distJoint;
    public Vector2 hookingPos;
    public bool hookableNear;

    //Efectos de las habilidades
    bool rolling = false;
    bool dashing = false;
    bool planing = false;
    bool goingUp = false;
    bool hooking = false;
    private bool haventDashed = false;
    private bool haventPlaned = false;
    private bool haventDoubleJump = false;




    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        distJoint.enabled = false;
        line.enabled = false;
    }

    void Update()
    {

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            lookingRight = true;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            lookingRight = false;
        }

        currentVel = Mathf.Lerp(currentVel, Input.GetAxisRaw("Horizontal") * velocidadX, accX * Time.deltaTime);
        body.velocity = new Vector2(currentVel, body.velocity.y);



        if (timeForDash > 0 && !grounded && dashing)
        {
            timeForDash -= Time.deltaTime;
        }
        else
        {
            dashing = false;
            timeForDash = StartTimeDash;
        }



        if (timeForPlane > 0 && !grounded && planing)
        {
            timeForPlane -= Time.deltaTime;
            
            if (timeForPlane < 1.3f)
            {
                goingUp = false;
            }
        }
        else
        {
            planing = false;
            timeForPlane = StartTimePlane;
        }



        if (Input.GetKeyDown(special) && hookableNear && canHook && !grounded)
        {
            distJoint.connectedAnchor = hookingPos;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hookingPos); 
            distJoint.enabled = true;
            line.enabled = true;
        }
        else if (Input.GetKeyUp(special) && canHook)
        {
            line.enabled = false;
            distJoint.enabled = false;
        }

        if (distJoint.enabled)
        {
            line.SetPosition(0, transform.position);
        }

        if (canRoll && climb){
            
            currentVel = Mathf.Lerp(currentVel, Input.GetAxisRaw("Horizontal") * velocidadX, accX * Time.deltaTime);
            body.velocity = new Vector2(body.velocity.x, Mathf.Abs( currentVel)/1.5f);
            rolling = true;
        }
        else
        {
            rolling = false;
        }


        if (Input.GetKeyDown(special) && !grounded && !haventDoubleJump && canDoubleJump)
        {
            haventDoubleJump = true;
            body.velocity = new Vector2(body.velocity.x, velocidady);
        }

        if (Input.GetKeyDown(special) && !grounded && !haventPlaned && canPlane)
        {
            goingUp = true;
            haventPlaned = true;
            planing = true;
        }

        if (Input.GetKeyDown(special) && !grounded && !haventDashed && canDash)
        {
            haventDashed = true;
            dashing = true;
        }



        if (dashing && timeForDash >= 0)
        {
            if (lookingRight)
            {
                Debug.Log("Dash R");
                body.velocity = new Vector2(dashVel, body.velocity.y);

            }
            else
            {
                Debug.Log("Dash L");
                body.velocity = new Vector2(-dashVel, body.velocity.y);
            }

        }

        if (planing && timeForPlane >= 0)
        {
            if (goingUp)
            {
                Debug.Log("Up!");
                body.velocity = new Vector2(body.velocity.x , planeVelUp);
            }
            else
            {
                Debug.Log("Down!");
                body.velocity = new Vector2(body.velocity.x, -planeVelDown);
            }



        }



        if (Input.GetKey(jump) && grounded ) 
        {
            if (!rolling)
            {

                body.velocity = new Vector2(body.velocity.x, velocidady);
            }
        }

        if (Input.GetKey(fall) && !grounded)
        {
            body.velocity = new Vector2(body.velocity.x, -fallingImprove);
        }


        if (climb && !canClimb)
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
            if (collision.collider.tag == "Escalable")
            {
                if (canRoll)
                {
                    canClimb = true;
                }
                climb = true;
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Suelo" || collision.collider.tag == "Escalable")
        {
            //grounded = false;
            Debug.Log("Registered out");
            currentCollisions--;
            haventDashed = false;
            haventPlaned = false;
            haventDoubleJump = false;
            if (currentCollisions == 0)
            {
                grounded = false;
            }

            if (collision.collider.tag == "Escalable")
            {
                climb = false;
            }
        }
       
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hookable")
        {
            hookableNear = true;
            hookingPos = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hookable")
        {
            hookableNear = false;
        }
    }
}
