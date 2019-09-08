using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Sheep : MonoBehaviour
{

    [ReadOnly] public int owner = 0;
    [ReadOnly] public int territory = 0; //0 - neutral, 1 - player1's, 2 - player2's S
    public float colliderDetectRadius = 1;
    public float drownHeightThreshold = -2;
    public bool hideGizmos = false;
    [ReadOnly] public float sheepStrength = 1;
    public float feedSpeed = 0.35f;

    Quaternion startBackOnFoursAngle = Quaternion.identity;
    Quaternion finalBackOnFoursAngle = Quaternion.identity;
    bool backOnFoursLerp;


    [Header("Sheeps update")]
    [ReadOnly] public float sheepUpdateTimer = 0; //Counting to one (eg.) second, in this time all sheeps will be updated
    public float sheepsUpdateTime = 1.0f; //One second

    [Header("Sheeps back on fours")]
    [ReadOnly] public float sheepBackOnFoursTimer = 0; //Counting to one (eg.) second, in this time all sheeps will be updated
    public float sheepspBackOnFoursTime = 3.0f; //One second


    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (sheepUpdateTimer < sheepsUpdateTime) //One second
        {
            sheepUpdateTimer += 1 * Time.deltaTime;
        }
        else
        {
            CheckDrown();
            CheckTerritory();
            BackOnFours();
            sheepUpdateTimer = 0;
            DamageTerritory();
            Feed();
        }

        if (sheepBackOnFoursTimer < sheepspBackOnFoursTime) //One second
        {
            sheepBackOnFoursTimer += 1 * Time.deltaTime;
            BackOnFoursLerp();
        }
    }

    [Button]
    public void BackOnFours()
    {
        if(sheepBackOnFoursTimer > sheepspBackOnFoursTime)
        {
            if (transform.localEulerAngles.x > 40 || transform.localEulerAngles.x < -40 || transform.localEulerAngles.z > 40 || transform.localEulerAngles.z < -40)
            {
                startBackOnFoursAngle = transform.rotation;
                finalBackOnFoursAngle = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

                if (rb.velocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
                {
                    sheepBackOnFoursTimer = 0;
                    transform.position += new Vector3(0, 1, 0);
                }
               
            }
        }
       
    }

    void BackOnFoursLerp()
    {
       // transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f,2.0f,0f), sheepBackOnFoursTimer * 2.5f);
        transform.rotation = Quaternion.Lerp(startBackOnFoursAngle, finalBackOnFoursAngle, sheepBackOnFoursTimer * 1.5f);
    }


    void CheckTerritory()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderDetectRadius);

        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if(hitColliders[i].transform.tag == "TerritoryPlayer1")
            {
                territory = 1;
                return;
            }
            else if (hitColliders[i].transform.tag == "TerritoryPlayer2")
            {
                territory = 2;
                return;
            }
            else if (hitColliders[i].transform.tag == "TerritoryNeutral")
            {
                territory = 3;
                return;
            }
            else
            {
                territory = 0;
            }
        }
    }

    void CheckDrown()
    {
        if (transform.localPosition.y < drownHeightThreshold)
        {
            Die();
        }
    }

    void Die()
    {
        MightyGamePack.MightyGameManager.gameManager.sheepDrownToSpawn++;
        Destroy(gameObject);
    }
   
    void DamageTerritory()
    {
        if (owner == 1 && territory == 2)
        {
            MightyGamePack.MightyGameManager.gameManager.healthPlayer2 -= Mathf.FloorToInt(sheepStrength);
            return;
        }

        if (owner == 2 && territory == 1)
        {
            MightyGamePack.MightyGameManager.gameManager.healthPlayer1 -= Mathf.FloorToInt(sheepStrength);
            return;
        }
    }

    void Feed()
    {
        if (territory == 3)
        {
            if(sheepStrength < 3)
            {
                sheepStrength += feedSpeed;
            }          
        }
        if (sheepStrength > 1) { transform.localScale = Vector3.one + new Vector3(sheepStrength, sheepStrength, sheepStrength) / 3; }
    }

    void OnDrawGizmos()
    {
        if(!hideGizmos)
        {
            DebugExtension.DrawCircle(transform.position, Vector3.up, Color.yellow, colliderDetectRadius);
            DebugExtension.DrawCircle(transform.position, Vector3.right, Color.yellow, colliderDetectRadius);
            DebugExtension.DrawCircle(transform.position, Vector3.forward, Color.yellow, colliderDetectRadius);
        }
      
    }
    
}