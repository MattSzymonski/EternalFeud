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
    public float shoutStrength;
    public float directionAngle;
  
    public float halfExtentsOfShoutBox = 1.0f;
    [Range(0, .3f)]
    public float movementSmoothing = 0.05f;


    public ParticleSystem chargingPS1;
    public ParticleSystem chargingPS2;


    Animator anim;
    public float drownHeightThreshold = -2;



    Vector3 lookDirection;
    Vector3 movementDirection;

    public float movementSpeed = 5;
    float shoutTimer;
    public float shoutTime = 2;
    public bool readyToShout = false;

    public Renderer stoneRenderer;
    public TransformJuicer flaotingJuicer;

    bool pp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponentInChildren<ParticleSystem>();
        effectJuicer = GetComponent<TransformJuicer>();
        shoutArea = GetComponentsInChildren<Transform>().Where(col => col.tag == "ShoutArea").SingleOrDefault();
        gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        anim = GetComponentInChildren<Animator>();
        Vector3 shoutPosition = shoutArea.transform.position;
        shoutPosition.z += 2.5f; //for now just twice wide as player
        shoutArea.transform.position = shoutPosition;
        flaotingJuicer.StartJuicing();
    }

    void Update()
    {
        if (gameManager.gameState == GameState.Playing)
        {
            Move();
            Rotation();
            Shout();
            CheckDrown();

            if(playerNumber == 1)
            {
                stoneRenderer.sharedMaterial.SetColor("_EmissiveColor", new Color(1f, 0.2f, 0.2f, 1) * shoutTimer * 60);
            }
            if (playerNumber == 2)
            {
                stoneRenderer.sharedMaterial.SetColor("_EmissiveColor", new Color(0.2f, 0.2f, 1f, 1) * shoutTimer * 60);
            }
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
        if (playerNumber == 1)
        {
            movementDirection = new Vector3(Input.GetAxis("Controller1 Left Stick Horizontal"), 0, -Input.GetAxis("Controller1 Left Stick Vertical")) * movementSpeed;
            DebugExtension.DebugArrow(transform.position, movementDirection);
            float yVel = rb.velocity.y;
            rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }
        if (playerNumber == 2)
        {
            movementDirection = new Vector3(Input.GetAxis("Controller2 Left Stick Horizontal"), 0, -Input.GetAxis("Controller2 Left Stick Vertical")) * movementSpeed;
            DebugExtension.DebugArrow(transform.position, movementDirection);
            float yVel = rb.velocity.y;
            rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }
    }
   


    void Rotation() // Calculating angle between player joystick right stick declension
    {
        if (playerNumber == 1)
        {
            lookDirection = new Vector3(Input.GetAxis("Controller1 Right Stick Horizontal"), 0, -Input.GetAxis("Controller1 Right Stick Vertical"));
            if (lookDirection == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
            DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
        }
        if (playerNumber == 2)
        {
            lookDirection = new Vector3(Input.GetAxis("Controller2 Right Stick Horizontal"), 0, -Input.GetAxis("Controller2 Right Stick Vertical"));
            if (lookDirection == Vector3.zero)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
            DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
        }
    }

    public void Shout()
    {
        if (playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Triggers") != 0)
            {
                if (!pp)
                {
                    chargingPS1.Play();
                    chargingPS2.Play();
                    pp = true;
                }
                
                if(!MightyGamePack.MightyGameManager.gameManager.audioManager.IsSoundPlaying("accumulate1"))
                {
                    MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound("accumulate1");
                }
                if (shoutTimer < shoutTime)
                {
                    shoutTimer += 1 * Time.deltaTime;
                   // chargingPS1.startColor = new Color(0.1f, 0.65f, 0.95f);
                   // chargingPS1.startColor = new Color(0.1f, 0.65f, 0.95f);
                 
                }
                else
                {
                    if (!readyToShout)
                    {
                        effectJuicer.StartJuicing();               
                    }
                    readyToShout = true;
                }
            }

            if (Input.GetAxis("Controller1 Triggers") == 0)
            {
                if (readyToShout)
                {
                    ShoutImpl();
                    readyToShout = false;
                }


       
                    chargingPS1.Stop();
                    chargingPS2.Stop();
                chargingPS1.Clear();
                chargingPS2.Clear();
                pp = false;
                

                shoutTimer = 0;

            }
        }

        if (playerNumber == 2)
        {
            if (Input.GetAxis("Controller2 Triggers") != 0)
            {
                if (!pp)
                {
                    chargingPS1.Play();
                    chargingPS2.Play();
                    pp = true;
                }
                if (!MightyGamePack.MightyGameManager.gameManager.audioManager.IsSoundPlaying("accumulate2"))
                {
                    MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound("accumulate2");
                }
                if (shoutTimer < shoutTime)
                {
                    shoutTimer += 1 * Time.deltaTime;
                 //   chargingPS1.startColor = new Color(1.0f, 0.5f, 0.5f);
                 //   chargingPS2.startColor = new Color(1.0f, 0.5f, 0.5f);
                    
                }
                else
                {
                    if (!readyToShout)
                    {
                        effectJuicer.StartJuicing();
                    }
                    readyToShout = true;
                }
            }

            if (Input.GetAxis("Controller2 Triggers") == 0)
            {
                if (readyToShout)
                {
                    ShoutImpl();
                    readyToShout = false;
                }


                chargingPS1.Stop();
                chargingPS2.Stop();
                chargingPS1.Clear();
                chargingPS2.Clear();
                pp = false;
                shoutTimer = 0;
            }
        }
    }
 
   private void ShoutImpl()
    {
        Vector3 boxBounds = transform.localScale * 8.0f;
        MightyGamePack.MightyGameManager.gameManager.audioManager.StopSound(playerNumber == 1 ? "accumulate1" : "accumulate2");
        MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound(playerNumber == 1 ? "whoosh1" : "whoosh2");
        foreach (var obj in Physics.OverlapBox(shoutArea.transform.position, boxBounds / 2.0f, transform.rotation)) //slightly bigger than current gizmo, when tweaking remember to tweak corresponding gizmo
        {     
            if (obj.tag != "Sheep") continue; // for now ignore shouting at other player

            float distanceX = Mathf.Abs(transform.position.x - obj.transform.position.x);
            float distanceZ = Mathf.Abs(transform.position.z - obj.transform.position.z);
            float distanceRoot = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);
            float shoutDecrease = Mathf.Pow((distanceRoot + 6), -2.0f) * 50.0f;

            lookDirection.y += directionAngle;
           // obj.GetComponent<Rigidbody>().AddForce(lookDirection * shoutStrength * shoutDecrease, ForceMode.Impulse);
            obj.GetComponent<Rigidbody>().AddForce(lookDirection * shoutStrength * Random.Range(0.9f,1.1f), ForceMode.Impulse);
            obj.GetComponent<Rigidbody>().AddTorque(GenerateRandomRotation() * 0.3f, ForceMode.Impulse);
            if(Random.Range(0, 100) > 90)
            {
                MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound("bleatexclusive");
            }
            else
            {
                MightyGamePack.MightyGameManager.gameManager.audioManager.PlayRandomSound("bleat1", "bleat2", "bleat3", "bleat4");
            }
            MightyGamePack.MightyGameManager.gameManager.particleEffectsManager.SpawnParticleEffect(obj.transform.position, Quaternion.identity, 20, 0.25f, "Hit");



           // Debug.Log("FUS RO DAH! on: " + obj.name);
        }
        particles.Play();
        Camera.main.transform.parent.GetComponent<MightyGamePack.CameraShaker>().ShakeOnce(3.0f, 1f, 1f, 1.25f);
        MightyGamePack.MightyGameManager.gameManager.UIManager.TriggerHitBlinkEffect(new Color(1, 1f, 1f, 0.05f));
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
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale * 8);
    }
}
