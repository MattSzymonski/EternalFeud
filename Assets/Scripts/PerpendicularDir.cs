using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MightyGamePack;

public class PerpendicularDir : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform point1;
    public Transform point2;

    void Start()
    {
        
    }

    
    void Update()
    {
        Debug.DrawLine(point1.position, point2.position);
        


        

    }



    private void OnDrawGizmos()
    {
        //Vector3 middlePoint = Vector3.Lerp(point1.position, point2.position, 0.5f);

        Vector3 middlePoint = point1.position - (point1.position - point2.position) * 0.5f;

        Gizmos.DrawLine(point1.position, point2.position);
        Gizmos.DrawSphere(MightyUtilites.ClearY(middlePoint), 0.5f);
        Gizmos.DrawLine(MightyUtilites.ClearY(middlePoint), MightyUtilites.ClearY(middlePoint) + MightyUtilites.PerpendicularToLine(point1.position, point2.position) * 10);
    }
}
