using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOverlap : OverlapChecker
{
    [Header("Box")]
    [SerializeField] private Vector3 size;

    protected override bool HandleDetection()
    {
        hitCount = Physics.OverlapBoxNonAlloc(pos, size / 2, colliders, transform.localRotation, layerMask);
        return hitCount != 0;
    }

    public override float GetSizeX()
    {
        return size.x;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(localOffset, size);
    }
}
