using MightyGamePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    MightyGameManager gameManager;

    Rigidbody rb;
    Transform headTransform;
    ParticleSystem particles;

    //-------Movement--------//
    [Header("Settings")]
    public int playerNumber;
    public float baseMoveSpeed;
    public float stoppingSpeed;
    Vector3 moveDirection;
    float moveSpeed;
    public bool moving;
    bool movingInput;
    bool rotatingInput;
    Vector3 velocity = Vector3.zero;
    [Range(0, .3f)]
    public float movementSmoothing = 0.05f;

    //Knockback
    public bool knockbacked;
    float knockbackTimer;
    float knockbackStrength;
    float knockbackLength;
    Vector3 knockbackDirection;

    //Dodge
    float dodgeTimer;
    public float dodgeDuration;
    float dodgeMoveSpeed;

    float dodgeCooldownTimer;
    public float dodgeCooldown;
    bool dodged;

    Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        headTransform = gameObject.transform.GetChild(0).GetComponent<Transform>();
        particles = GetComponentInChildren<ParticleSystem>();
        moveSpeed = baseMoveSpeed;
        gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (gameManager.gameState == GameState.Playing)
        {
            Move();
         //   Dodge(); //Add later
            Rotation();
            Shout();
        }
    }


    void Move() //Interpreting player controllers input
    {
        float moveSpeedRel = 1.0f;
        if(playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Left Stick Horizontal") != 0 && Input.GetAxis("Controller1 Left Stick Vertical") != 0) { moveSpeedRel = moveSpeed * 0.7071f + dodgeMoveSpeed; } else { moveSpeedRel = moveSpeed + dodgeMoveSpeed; }
            if (Input.GetAxis("Controller1 Left Stick Horizontal") != 0 || Input.GetAxis("Controller1 Left Stick Vertical") != 0) { movingInput = true; } else { movingInput = false; }

            if (!knockbacked)
            {
                if (Input.GetAxis("Controller1 Left Stick Horizontal") < 0 ) { moveDirection.x = -1; }
                else if(Input.GetAxis("Controller1 Left Stick Horizontal") > 0 ){ moveDirection.x = 1; }
                if (Input.GetAxis("Controller1 Left Stick Vertical") < 0 ) { moveDirection.z = 1; }
                else if(Input.GetAxis("Controller1 Left Stick Vertical") > 0 ) { moveDirection.z = -1; }
            }
           // Debug.Log("direction x: " + moveDirection.x +  "direction z: " + moveDirection.z);
        }
        else if(playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Left Stick Horizontal") != 0 && Input.GetAxis("Controller2 Left Stick Vertical") != 0) { moveSpeedRel = moveSpeed * 0.7071f + dodgeMoveSpeed; } else { moveSpeedRel = moveSpeed + dodgeMoveSpeed; }
            if (Input.GetAxis("Controller2 Left Stick Horizontal") != 0 || Input.GetAxis("Controller2 Left Stick Vertical") != 0) { movingInput = true; } else { movingInput = false; }

            if (!knockbacked)
            {
                if (Input.GetAxis("Controller2 Left Stick Horizontal") < 0) { moveDirection.x = -1; }
                else if (Input.GetAxis("Controller2 Left Stick Horizontal") > 0) { moveDirection.x = 1; }
                if (Input.GetAxis("Controller2 Left Stick Vertical") < 0) { moveDirection.z = 1; }
                else if (Input.GetAxis("Controller2 Left Stick Vertical") > 0) { moveDirection.z = -1; }
            }
        }

        if ((rb.velocity.x != 0 || rb.velocity.y != 0) && !knockbacked)
        {
            // anim.SetBool("Movement", true);
            moving = true;
        }
        else
        {
            //anim.SetBool("Movement", false);
            moving = false;
        }
        // anim.SetBool("Knockbacked", knockbacked);
        if (moveDirection.x < 0) { moveDirection.x += stoppingSpeed * Time.deltaTime; }
        if (moveDirection.x > 0) { moveDirection.x -= stoppingSpeed * Time.deltaTime; }
        if (moveDirection.z < 0) { moveDirection.z += stoppingSpeed * Time.deltaTime; }
        if (moveDirection.z > 0) { moveDirection.z -= stoppingSpeed * Time.deltaTime; }

        if (moveDirection.x > -0.1f && moveDirection.x < 0.1f) { moveDirection.x = 0; }
        if (moveDirection.z > -0.1f && moveDirection.z < 0.1f) { moveDirection.z = 0; }



        rb.velocity = moveDirection * moveSpeedRel;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, moveDirection * moveSpeedRel, ref velocity, movementSmoothing);
    }
    void Dodge()
    {
        dodgeCooldownTimer += 3 * Time.fixedDeltaTime;
        dodgeTimer += 3 * Time.fixedDeltaTime;

        if (dodgeCooldownTimer > dodgeCooldown)
        {
            if (Input.GetButtonDown("Space"))
            {
                dodgeTimer = 0;
                dodged = true;
                // dodgeParticles.Play();
            }
        }

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && dodgeTimer < dodgeDuration) { dodgeMoveSpeed = 5; } else { dodgeMoveSpeed = 0; }// dodgeParticles.Stop(); }

        if (dodged)
        {
            if (Input.GetButtonUp("Space"))
            {
                dodgeCooldownTimer = 0;
                dodged = false;
            }
        }


    }

    void Rotation() // Calculating angle between player joystick right stick declension
    {
        float XAxis = 0, YAxis = 0, angle = 0;
        if(playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Right Stick Horizontal") != 0 || Input.GetAxis("Controller1 Right Stick Vertical") != 0) { rotatingInput = true; } else { rotatingInput = false; }
            if (rotatingInput)
            {
                XAxis = Input.GetAxis("Controller1 Right Stick Horizontal");
                YAxis = Input.GetAxis("Controller1 Right Stick Vertical");
            }
        } else if(playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Right Stick Horizontal") != 0 || Input.GetAxis("Controller2 Right Stick Vertical") != 0) { rotatingInput = true; } else { rotatingInput = false; }
            if (rotatingInput)
            {
                XAxis = Input.GetAxis("Controller2 Right Stick Horizontal");
                YAxis = Input.GetAxis("Controller2 Right Stick Vertical");
            }
        }


        angle = Mathf.Round(Mathf.Atan2(XAxis, YAxis) * Mathf.Rad2Deg); //for now no drag applied
        if (angle < 0) { angle += 360f; }
        headTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    public void Shout()
    {
        if(playerNumber == 1)
        {
            if(Input.GetAxis("Controller1 Triggers") != 0)
            {
                Debug.Log("FUS RO DAH!!!!");
                particles.Play();
            }
        } else if(playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Triggers") != 0)
            {
                Debug.Log("FUS RO DAH!!!!");
            }

        }
    }
}
