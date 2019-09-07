using MightyGamePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    MightyGameManager gameManager;

    Rigidbody rb;
    ParticleSystem particles;
    Transform shoutArea;
    TransformJuicer effectJuicer;

    //-------Movement--------//
    [Header("Settings")]
    public int playerNumber;
    public float baseMoveSpeed;
    public float stoppingSpeed;
    public float shoutStrength;
    public float directionAngle;
    public float shoutCooldown;
    Vector3 moveDirection;
    float moveSpeed;
    public bool moving;
    bool movingInput;
    bool rotatingInput;
    Vector3 velocity = Vector3.zero;
    public float halfExtentsOfShoutBox = 1.0f;
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
    public float drownHeightThreshold = -2;

    private float currentAngle;
    private bool shoutReady = false;
    private bool shownVisualCue = false;
    private float shoutTimeAccumulate = 0.0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponentInChildren<ParticleSystem>();
        effectJuicer = GetComponent<TransformJuicer>();
        shoutArea = GetComponentsInChildren<Transform>().Where(col => col.tag == "ShoutArea").SingleOrDefault();
        moveSpeed = baseMoveSpeed;
        gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        anim = GetComponentInChildren<Animator>();
        Vector3 shoutPosition = shoutArea.transform.position;
        shoutPosition.z += 2.5f; //for now just twice wide as player
        shoutArea.transform.position = shoutPosition;
    }

    void Update()
    {
        if (gameManager.gameState == GameState.Playing)
        {
            Move();
         //   Dodge(); //Add later
            Rotation();
            Shout();
            CheckDrown();
        }
    }

    void CheckDrown()
    {
        if (transform.position.y < drownHeightThreshold)
        {
            if (playerNumber == 1)
            {
                MightyGameManager.gameManager.GameOver(2);
            }
            if (playerNumber == 2)
            {
                MightyGameManager.gameManager.GameOver(1);
            }


        }
    }



    void Move() //Interpreting player controllers input
    {
        float moveSpeedRel = 0.5f;
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
        float XAxis = 0, ZAxis = 0;
        if(playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Right Stick Horizontal") != 0 || Input.GetAxis("Controller1 Right Stick Vertical") != 0) { rotatingInput = true; } else { rotatingInput = false; }
            if (rotatingInput)
            {
                ZAxis = Input.GetAxis("Controller1 Right Stick Horizontal");
                XAxis = Input.GetAxis("Controller1 Right Stick Vertical");
            }
        } else if(playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Right Stick Horizontal") != 0 || Input.GetAxis("Controller2 Right Stick Vertical") != 0) { rotatingInput = true; } else { rotatingInput = false; }
            if (rotatingInput)
            {
                ZAxis = Input.GetAxis("Controller2 Right Stick Horizontal");
                XAxis = Input.GetAxis("Controller2 Right Stick Vertical");
            }
        }

        currentAngle = Mathf.Round(Mathf.Atan2(ZAxis, XAxis) * Mathf.Rad2Deg); //for now no drag applied
        currentAngle += 90.0f;
        if (currentAngle < 0) { currentAngle += 360f; } else if(currentAngle >= 360.0f) { currentAngle -= 360.0f; }
        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
    }

    public void Shout()
    {
        if(playerNumber == 1)
        {
            if(Input.GetAxis("Controller1 Triggers") != 0)
            {
                UpdateTimers();
            }
            else if (shoutReady)
            {
                shoutTimeAccumulate = 0.0f;
                shownVisualCue = false;
                ShoutImpl();
            }
            else
            {
                //can tweak later
                shoutTimeAccumulate = 0.0f;
                shownVisualCue = false;
            }   
        } else if(playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Triggers") != 0)
            {
                UpdateTimers();
            }
            else if (shoutReady)
            {
                shoutTimeAccumulate = 0.0f;
                shownVisualCue = false;
                ShoutImpl();
            }
            else
            {
                //can tweak later
                shoutTimeAccumulate = 0.0f;
                shownVisualCue = false;
            }
        }
    }

    public void Restart()
    {
        shoutReady = false;
        shownVisualCue = false;
        shoutTimeAccumulate = 0.0f;
    }

    private void UpdateTimers()
    {
        shoutTimeAccumulate += Time.deltaTime;
        if (shoutTimeAccumulate >= shoutCooldown)
        {
            shoutReady = true;
        }
        if(shoutReady && !shownVisualCue)
        {
            effectJuicer.StartJuicing();
            shownVisualCue = true;
        }
    }
    
    private void ShoutImpl()
    {
        shoutReady = false;
        //for now just react for every object in collision regardless of distance from source
        //TODO: add cooldown
        // add reduction of power further from me
        // ignore objects not in cone -> add additional box for objects close by
        // add visual cue that it is ready
        Vector3 boxBounds = transform.localScale * 4.0f;
        foreach (var obj in Physics.OverlapBox(shoutArea.transform.position, boxBounds / 2.0f, transform.rotation)) //slightly bigger than current gizmo, when tweaking remember to tweak corresponding gizmo
        {
            if (obj.tag != "Sheep") continue; // shout only at sheep
            Vector3 direction = transform.rotation * Vector3.forward;
            direction.y = directionAngle;

            float distanceX = Mathf.Abs(transform.position.x - obj.transform.position.x);
            float distanceZ = Mathf.Abs(transform.position.z - obj.transform.position.z);
            float distanceRoot = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
            float shoutDecrease = Mathf.Pow((distanceRoot + 1), -2.0f) * 10.0f;

            obj.GetComponent<Rigidbody>().AddForce(direction * shoutStrength * shoutDecrease, ForceMode.Impulse);
            obj.GetComponent<Rigidbody>().AddTorque(GenerateRandomRotation() * 0.3f, ForceMode.Impulse);
        }
        Debug.Log("FUS RO DAH!!!!");
        particles.Play();
        //add visual cue
    }

    private Vector3 GenerateRandomRotation()
    {
        int rotateX = Random.Range(0, 2);
        int rotateY = Random.Range(0, 2);
        int rotateZ = Random.Range(0, 2);
        return new Vector3(rotateX, rotateY, rotateZ);
    }

    private void OnDrawGizmos()
    {
        if (!shoutArea) return;
        Gizmos.matrix = Matrix4x4.TRS(shoutArea.transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale * 4);
    }
}
