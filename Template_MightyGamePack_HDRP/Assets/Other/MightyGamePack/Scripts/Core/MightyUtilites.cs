using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MightyGamePack
{
    public class MightyUtilites
    {
        public static Vector3 ClearY(Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        public static Vector3 PerpendicularToLine(Vector3 lineStartPoint, Vector3 lineEndPoint)
        {
            Vector3 lineDir = lineStartPoint - lineEndPoint;
            Vector3 perpendicularDir = Vector3.Cross(lineDir, Vector3.up).normalized;
            return perpendicularDir;
        }
    }
}