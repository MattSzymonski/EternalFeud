using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Sheep : MonoBehaviour
{

    [ReadOnly]
    public int territory = 0; //0 - neutral, 1 - player1's, 2 - player2's S
    public float colliderDetectRadius = 1;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void CheckTerritory()
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
            else
            {
                territory = 0;
            }
        }
    }



}
