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

    public Quaternion startBackOnFoursAngle;
    bool backOnFoursLerp;


    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {

        if(backOnFoursLerp)
        {
            BackOnFoursLerp();
        }
    }

    [Button]
    public void BackOnFours()
    {
        if (transform.localEulerAngles.x > 30 || transform.localEulerAngles.x < -30 || transform.localEulerAngles.z > 30 || transform.localEulerAngles.z < -30)
        {
            startBackOnFoursAngle = transform.rotation;
            
            if (rb.velocity.magnitude < 0.01f)
            {
                transform.position += new Vector3(0, 2, 0);
            }
            
            
            backOnFoursLerp = true;


           

        }
    }

    void BackOnFoursLerp()
    {
        transform.rotation = Quaternion.Lerp(startBackOnFoursAngle,Quaternion.EulerRotation(0,0,0) , Time.time * 0.4f);
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
        if (transform.position.y < drownHeightThreshold)
        {
            Die();
        }
    }

    void Die()
    {
        MightyGamePack.MightyGameManager.gameManager.sheepToUpdate--;
        MightyGamePack.MightyGameManager.gameManager.cleanupSheepList = true;
        MightyGamePack.MightyGameManager.gameManager.sheepDrownToSpawn++;
        Destroy(gameObject);
    }

    public void UpdateSheep()
    {
       // Debug.Log(transform.name);
        CheckDrown();
        CheckTerritory();
        BackOnFours();
    }

    /*
   void StandUp()
   {

   }

   */

   
    void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, Vector3.up, Color.yellow, colliderDetectRadius);
        DebugExtension.DrawCircle(transform.position, Vector3.right, Color.yellow, colliderDetectRadius);
        DebugExtension.DrawCircle(transform.position, Vector3.forward, Color.yellow, colliderDetectRadius);
    }
    
}