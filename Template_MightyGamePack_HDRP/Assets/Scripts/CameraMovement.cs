using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject[] playerObjects;
    Transform[] players;
    public float horizontalPanning = 1.0f;
    public float verticalPanning = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        players = new Transform[2];
        for (int i = 0; i < playerObjects.Length; ++i)
        {
            if (playerObjects[i].GetComponent<PlayerMovement>().playerNumber == 1)
            {
                players[0] = playerObjects[i].transform;
            }
            else if (playerObjects[i].GetComponent<PlayerMovement>().playerNumber == 2)
            {
                players[1] = playerObjects[i].transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Length == 0) //not yet setup or wrong 
            return;
        Vector3 midPoint = new Vector3(0.0f, 0.0f, 0.0f);
        // find a middlepoint between two players and look at it with some angle
        for(int i = 0; i < players.Length; ++i)
        {
            midPoint += players[i].transform.position;
        }
        midPoint /= players.Length;
        // add special panning (some randomness)

        transform.LookAt(midPoint);
    }

}
