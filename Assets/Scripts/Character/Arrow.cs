using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ArrowHit
{
    public Arrow Owner;
    public Collider Collider;
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public Vector3 HitDirection;
    public float distance;

    public void Set(Arrow owner, Collider collider, Vector3 hitPoint, Vector3 hitNormal, Vector3 hitDirection)
    {
        Owner = owner;
        Collider = collider;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
        HitDirection = hitDirection;
    }
}

public class Arrow : MonoBehaviour
{
    [SerializeField] LayerMask _collisionLayer;
    [SerializeField] Vector3 _detectOffset;
    [SerializeField] float _detectRadius;

    private Vector3 _direction;
    private float _speed;
    private bool _isHit;

    private void Awake()
    {

    }

    public void Init(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
        transform.forward = direction;
    }

    public bool RayTraceForward (out ArrowHit hit)
    {
        Collider[] collider = Physics.OverlapSphere(GetOffsetPosition(), _detectRadius, _collisionLayer);
        if (collider.Length > 0)
        {
            hit = new ArrowHit();
            hit.Set(this, collider[0], collider[0].ClosestPoint(transform.position), collider[0].ClosestPointOnBounds(transform.position), _direction);
            _isHit = true;
            return true;
        }
        else
        {
            hit = new ArrowHit();
            return false;
        }
    }

    private void Update()
    {
        if (_isHit)
            return;
        transform.position += _direction * _speed * Time.deltaTime;
        if (RayTraceForward(out ArrowHit hit))
        {
            OnHit(hit);
        }
    }

    private void OnHit(ArrowHit hit)
    {
        _isHit = true;
        transform.SetParent(hit.Collider.transform, true);
        //transform.forward = Vector3.Reflect(_direction, hit.HitNormal);
    }

    private Vector3 GetOffsetPosition()
    {
        return transform.position + transform.forward * _detectOffset.z + transform.right * _detectOffset.x + transform.up * _detectOffset.y;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (ExisingCollisionLayer (other.gameObject.layer))
    //    {
    //        _isHit = true;
    //        transform.SetParent(other.transform);
    //    }
    //}

    public bool ExisingCollisionLayer(int layer)
    {
        return (_collisionLayer & (1 << layer)) != 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetOffsetPosition(), _detectRadius);  
    }
}
