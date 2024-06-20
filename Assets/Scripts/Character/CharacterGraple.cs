using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraple : MonoBehaviour
{
    public Vector3 CalculateJumpVelocity (Vector3 startPoint, Vector3 endPoint, float height, float gravity)
    {
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity));

        return velocityY + velocityXZ;
    }
}
