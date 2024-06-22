using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStunnable
{
    void Stun(float duration);
}

public class StunArrowModifier : MonoBehaviour
{
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _stunRadius;
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private ParticleSystem _stunEffect;

    private Arrow _arrow;

    private void Awake()
    {
        _arrow = GetComponent<Arrow>();
        _arrow.OnImpact += OnHit;
    }

    private void OnHit(ArrowHit hit)
    {
        Stun(hit);
    }

    private void Stun(ArrowHit hit)
    {
        Vector3 hitPoint = hit.HitPoint;
        Collider[] colliders = Physics.OverlapSphere(hitPoint, _stunRadius, _collisionLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IStunnable stunnable))
            {
                stunnable.Stun(_stunDuration);
            }
        }

        ObjectPoolManager.SpawnObject(_stunEffect, hitPoint, Quaternion.identity);
    }
}
