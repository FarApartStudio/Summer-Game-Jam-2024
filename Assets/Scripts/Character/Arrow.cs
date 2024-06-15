using Pelumi.Juicer;
using Pelumi.ObjectPool;
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
    public Action<ArrowHit> OnHit;

    [SerializeField] LayerMask _collisionLayer;
    [SerializeField] Vector3 _detectOffset;
    [SerializeField] float _detectRadius;
    [SerializeField] float _lifeTime = 20;
    [SerializeField] float _afterHitLifeTime = 5;

    [Header("Trail")]
    [SerializeField] float _trailHideDelay;
    [SerializeField] GameObject _trail;

    private Vector3 _direction;
    private float _speed;
    private bool _isHit;
    private JuicerRuntimeCore<Vector3> _scaleEffect;
    private Coroutine _deSpawnRoutine;

    private void Awake()
    {
        _scaleEffect = transform.JuicyScale(Vector3.one * 1.5f, 0.15f);
        _scaleEffect.SetLoop(2);
    }

    public void Init(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
        transform.forward = direction;
        _trail.gameObject.SetActive(true);
        _deSpawnRoutine = StartCoroutine(DeSpawn(_lifeTime));
        _isHit = false;
    }

    public bool RayTraceForward (out ArrowHit hit)
    {
        Collider[] collider = Physics.OverlapSphere(GetOffsetPosition(), _detectRadius, _collisionLayer);
        if (collider.Length > 0)
        {
            _isHit = true;
            hit = new ArrowHit();
            Vector3 hitDirection = (collider[0].transform.position - transform.position).normalized;
            hit.Set(this, collider[0], collider[0].ClosestPoint(transform.position), collider[0].ClosestPointOnBounds(transform.position), hitDirection);
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
            Hit(hit);
        }
    }

    private void Hit(ArrowHit hit)
    {
        OnHit?.Invoke(hit);
        transform.SetParent(hit.Collider.transform, true);

        IEnumerator HideTrail()
        {
            yield return new WaitForSeconds(_trailHideDelay);
            _trail.gameObject.SetActive(false);
        }

        // _scaleEffect.Start();

        StartCoroutine(HideTrail());

        if (_deSpawnRoutine != null)
            StopCoroutine(_deSpawnRoutine);
        _deSpawnRoutine = StartCoroutine(DeSpawn(_afterHitLifeTime));
    }


    IEnumerator DeSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        transform.SetParent(null);
        ObjectPoolManager.ReleaseObject(gameObject);
    }

    private Vector3 GetOffsetPosition()
    {
        return transform.position + transform.forward * _detectOffset.z + transform.right * _detectOffset.x + transform.up * _detectOffset.y;
    }

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
