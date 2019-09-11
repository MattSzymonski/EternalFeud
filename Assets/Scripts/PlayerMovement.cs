using MightyGamePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

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

    public Renderer stoneRenderer;
    public TransformJuicer floatingJuicer;
    public ParticleSystem chargingPS1;
    public ParticleSystem chargingPS2;

    Animator anim;
    Vector3 lookDirection;
    Quaternion previousLookRotation = Quaternion.identity; // when player releases axis, continue capturing old direction
    Vector3 movementDirection;
    bool pp = false;


    [Header("Movement")]
    public bool useMouseAndKeyboardInput;   
    public bool useGamePadInput;
    [ShowIf("useGamePadInput")] public int controllerNumber;

    public float movementSpeed = 5;
    [Range(0, .3f)]
    public float movementSmoothing = 0.05f;
    Vector3 mouse;
    Ray camRay;
    RaycastHit camHit;
    public string mouseTargetLayerName;
    int mouseTargetLayer;
    public bool mouseRayGizmo;
    public float drownHeightThreshold = -2;

    [Header("Shooting")]
    public float shoutStrengthForward;
    public float shoutStrengthUp;
    float shoutTimer;
    public float shoutTime = 2;
    [ReadOnly] public bool readyToShout = false;
    public bool applyKnockback = false;
    public float knockbackForce = 500.0f;

    void Start()
    {
        mouseTargetLayer = LayerMask.GetMask(mouseTargetLayerName);

        rb = GetComponent<Rigidbody>();
        particles = GetComponentInChildren<ParticleSystem>();
        effectJuicer = GetComponent<TransformJuicer>();
        shoutArea = GetComponentsInChildren<Transform>().Where(col => col.tag == "ShoutArea").SingleOrDefault();
        gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
        anim = GetComponentInChildren<Animator>();
        Vector3 shoutPosition = shoutArea.transform.position;
        shoutPosition.z += 2.5f; //for now just twice wide as player
        shoutArea.transform.position = shoutPosition;
        floatingJuicer.StartJuicing();
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
        if (useMouseAndKeyboardInput)
        {
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * movementSpeed;
            DebugExtension.DebugArrow(transform.position, movementDirection);
            float yVel = rb.velocity.y;
            rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }

        if (useGamePadInput)
        {
             movementDirection = new Vector3(Input.GetAxis("Controller" + controllerNumber + " Left Stick Horizontal"), 0, -Input.GetAxis("Controller" + controllerNumber + " Left Stick Vertical")) * movementSpeed;
             DebugExtension.DebugArrow(transform.position, movementDirection);
             float yVel = rb.velocity.y;
             rb.velocity = new Vector3(movementDirection.x, yVel, movementDirection.z);
        }
    }
   


    void Rotation() // Calculating angle between player joystick right stick declension
    {
        if (useMouseAndKeyboardInput)
        {
            camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out camHit, 5000, mouseTargetLayer))
            {
                Vector3 relativeDirection = camHit.point - transform.position;
                Vector3 moveDirection = MightyUtilites.ClearY(relativeDirection).normalized;

                if (Vector3.Distance(MightyUtilites.ClearY(transform.position), MightyUtilites.ClearY(camHit.point)) > 0.3f)
                {
                    Vector3 newRotation = Vector3.RotateTowards(transform.forward, moveDirection, 15.0f * Time.deltaTime, 0.0f).normalized;
                    transform.rotation = Quaternion.LookRotation(newRotation, Vector3.up);
                    lookDirection = newRotation;
                }
            }
        }

        if (useGamePadInput)
        {
            lookDirection = new Vector3(Input.GetAxis("Controller" + controllerNumber + " Right Stick Horizontal"), 0, -Input.GetAxis("Controller" + controllerNumber + " Right Stick Vertical")).normalized;
            if (lookDirection == Vector3.zero)
            {
                transform.rotation = previousLookRotation;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                previousLookRotation = transform.rotation;
            }

            DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
        }
    }

    public void Shout()
    {
        if (useMouseAndKeyboardInput)
        {
            if (Input.GetButton("Space"))
            {
                if (!pp)
                {
                    // chargingPS1.Play();
                    // chargingPS2.Play();
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

            if (!Input.GetButton("Space"))
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

        if (useGamePadInput)
        {
            if (Input.GetAxis("Controller" + controllerNumber + " Triggers") != 0)
            {
                if (!pp)
                {
                    // chargingPS1.Play();
                    //  chargingPS2.Play();
                    pp = true;
                }

                if (!MightyGamePack.MightyGameManager.gameManager.audioManager.IsSoundPlaying("accumulate1"))
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

            if (Input.GetAxis("Controller" + controllerNumber + " Triggers") == 0)
            {
                if (readyToShout)
                {
                    ShoutImpl();
                    readyToShout = false;
                }

                //chargingPS1.Stop();
                //chargingPS2.Stop();
                //chargingPS1.Clear();
                //chargingPS2.Clear();
                pp = false;

                shoutTimer = 0;
            }

        }



        /*
        if (playerNumber == 1)
        {
            if (Input.GetAxis("Controller1 Triggers") != 0)
            {
                if (!pp)
                {
                   // chargingPS1.Play();
                  //  chargingPS2.Play();
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

                //chargingPS1.Stop();
                //chargingPS2.Stop();
                //chargingPS1.Clear();
                //chargingPS2.Clear();
                pp = false;
               
                shoutTimer = 0;
            }
        }

        if (playerNumber == 2)
        {
            /*

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
            

            if (Input.GetButton("Space"))
            {
                if (!pp)
                {
                  //  chargingPS1.Play();
                 //   chargingPS2.Play();
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

            if (!Input.GetButton("Space"))
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
        */
    }

    private void ShoutImpl()
    {
       // Debug.Log(lookDirection);
        Vector3 boxBounds = transform.localScale * 8.0f;
        MightyGamePack.MightyGameManager.gameManager.audioManager.StopSound(playerNumber == 1 ? "accumulate1" : "accumulate2");
        MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound(playerNumber == 1 ? "whoosh1" : "whoosh2");
        float totalMassEjected = 0.0f;
        foreach (var obj in Physics.OverlapBox(shoutArea.transform.position, boxBounds / 2.0f, transform.rotation)) //slightly bigger than current gizmo, when tweaking remember to tweak corresponding gizmo
        {     
            if (obj.tag != "Sheep") continue; // for now ignore shouting at non sheep

            var objRb = obj.GetComponent<Rigidbody>();
            objRb.AddForce(new Vector3(lookDirection.x, shoutStrengthUp * Random.Range(0.8f, 1.3f), lookDirection.z) * shoutStrengthForward * Random.Range(0.8f, 1.2f), ForceMode.Impulse);
            objRb.AddTorque(GenerateRandomRotation() * 0.5f, ForceMode.Impulse);
            if(Random.Range(0, 100) > 93)
            {
                MightyGamePack.MightyGameManager.gameManager.audioManager.PlaySound("bleatexclusive");
            }
            else
            {
                MightyGamePack.MightyGameManager.gameManager.audioManager.PlayRandomSound("bleat1", "bleat2", "bleat3", "bleat4");
            }
            MightyGamePack.MightyGameManager.gameManager.particleEffectsManager.SpawnParticleEffect(obj.transform.position, Quaternion.identity, 20, 0.25f, "Hit");
            totalMassEjected += objRb.mass;

           // Debug.Log("FUS RO DAH! on: " + obj.name);
        }
        if(applyKnockback)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(-lookDirection.x, 0.0f, -lookDirection.z) * knockbackForce * totalMassEjected, ForceMode.Acceleration);
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
        if (mouseRayGizmo)
        {
            if(camHit.point != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(Camera.main.transform.localPosition, 0.5f);
                Gizmos.DrawLine(Camera.main.transform.position, camHit.point);
                Gizmos.DrawSphere(camHit.point, 0.5f);
            } 
        }

        Gizmos.color = Color.green;
        if (!shoutArea) return;
        Gizmos.matrix = Matrix4x4.TRS(shoutArea.transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale * 8);
    }
}
