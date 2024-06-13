using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OverlapChecker : MonoBehaviour
{
    public Action<int, Collider[]> OnDetect;


    [SerializeField] protected Color color;
    [SerializeField] protected Vector2 localOffset;
    [SerializeField] protected int maxHit = 10;
    [SerializeField] protected LayerMask layerMask;

    protected Collider[] colliders;
    protected int hitCount;
    protected Vector2 pos;

    public Vector2 Offset => localOffset;
    


    private void Awake()
    {
        colliders = new Collider[maxHit];
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        this.layerMask = layerMask;
    }

    protected abstract bool HandleDetection();

    public bool HasDetected()
    {
        pos = transform.position + transform.rotation * localOffset;
        return HandleDetection();
    }

    public bool DetectOverlap()
    {
        pos = transform.position + transform.rotation * localOffset;
        if (HandleDetection())
        {
            OnDetect?.Invoke(hitCount, colliders);
            return true;
        }
        return false;
    }

    public virtual float GetSizeX()
    {
        return 0;
    }
}
