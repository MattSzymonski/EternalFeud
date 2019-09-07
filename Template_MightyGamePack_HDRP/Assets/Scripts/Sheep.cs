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

    void Start()
    {
        
    }


    void CheckTerritory()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderDetectRadius);

       

        for (int i = 0; i < hitColliders.Length; i++)
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
            else
            {
                territory = 0;
                return;
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
        Destroy(gameObject);
    }

    public void UpdateSheep()
    {
        CheckDrown();
        CheckTerritory();
       
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