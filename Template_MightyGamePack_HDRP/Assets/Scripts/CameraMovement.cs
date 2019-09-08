using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 3.0f;
    public GameObject playerObject1;
    public GameObject playerObject2;
    MightyGamePack.MightyGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<MightyGamePack.MightyGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject1 != null && playerObject2 != null && gameManager.gameState == MightyGamePack.GameState.Playing)
        {
            Vector3 midPoint = new Vector3(0.0f, 0.0f, 0.0f);
            // find a middlepoint between two players and look at it with some angle
            midPoint += playerObject2.transform.position - new Vector3(0.0f, 0.0f, 5.0f); //special offset to decline camera    
            midPoint += playerObject1.transform.position -new Vector3(0.0f, 0.0f, 5.0f);

            midPoint /= 2;
            var step = rotationSpeed * Time.deltaTime;
            Vector3 relativePosition = midPoint - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, step);
        }
       
    }
}
