using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOverlap : OverlapChecker
{
    [Header("Sphere")]
    [SerializeField] private float size;

    protected override bool HandleDetection()
    {
        hitCount = Physics.OverlapSphereNonAlloc(pos, size, colliders, layerMask);
        return hitCount != 0;
    }

    public override float GetSizeX()
    {
        return size;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(localOffset , size);
    }
}
