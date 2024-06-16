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
    [SerializeField] int _maxHitCount = 2;

    [Header("Trail")]
    [SerializeField] float _trailHideDelay;
    [SerializeField] GameObject _trail;

    private Vector3 _direction;
    private float _speed;
    private bool _isHit;
    private bool _canMove = true;
    private JuicerRuntimeCore<Vector3> _scaleEffect;
    private Coroutine _deSpawnRoutine;
    private DamageDefector checkedDefector;

    [Header("Debug")]
    [SerializeField] private Collider[] hits;

    private void Awake()
    {
        _scaleEffect = transform.JuicyScale(Vector3.one * 1.5f, 0.15f);
        _scaleEffect.SetLoop(2);

        hits = new Collider[_maxHitCount];
    }

    public void Init(Vector3 direction, float speed)
    {
        _direction = direction;
        _speed = speed;
        transform.forward = direction;
        _trail.gameObject.SetActive(true);
        _deSpawnRoutine = StartCoroutine(DeSpawn(_lifeTime));
        _isHit = false;
        _canMove = true;
        checkedDefector = null;
    }

    public void Trace()
    {
        if (_isHit) return;

        int hitCount = Physics.OverlapSphereNonAlloc(GetOffsetPosition(), _detectRadius, hits, _collisionLayer);
        if (hitCount > 0)
        {
            ProcessHits (hitCount);
        }
    }

    private void Update()
    {
        if (_canMove)
        transform.position += _direction * _speed * Time.deltaTime;

        Trace();
    }

    public void ProcessHits(int hitCount)
    {
        for (int i = 0; i < hitCount; i++)
        {
            if (_isHit)
                break;

            if (hits[i] == null)
                continue;

            if (hits[i].TryGetComponent(out DamageDefector defector))
            {
                ProcessDefector (defector);
            }
            else
            {
                SuccesFulHit(hits[i]);
            }
        }
    }

    private void ProcessDefector (DamageDefector defector)
    {
        if (checkedDefector == defector)
        {
            return;
        }

        checkedDefector = defector;

        if (defector.Defect(transform.position))
        {
            _isHit = true;

            if (_deSpawnRoutine != null)
                StopCoroutine(_deSpawnRoutine);
            _deSpawnRoutine = StartCoroutine(DeSpawn(_afterHitLifeTime));
        }
    }

    private void SuccesFulHit(Collider collider)
    {
        _isHit = true;
        _canMove = false;

        ArrowHit hit = new ArrowHit();
        Vector3 hitDirection = (collider.transform.position - transform.position).normalized;
        hit.Set(this, collider, collider.ClosestPoint(transform.position), collider.ClosestPointOnBounds(transform.position), hitDirection);

        OnHit?.Invoke(hit);

        transform.SetParent(hit.Collider.transform, true);

        IEnumerator HideTrail()
        {
            yield return new WaitForSeconds(_trailHideDelay);
            _trail.gameObject.SetActive(false);
        }

        StartCoroutine(HideTrail());

        if (_deSpawnRoutine != null)
            StopCoroutine(_deSpawnRoutine);
        _deSpawnRoutine = StartCoroutine(DeSpawn(_afterHitLifeTime));
    }


    IEnumerator DeSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        _trail.gameObject.SetActive(false);
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
