using Pelumi.ObjectPool;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveArrowModifier : MonoBehaviour
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;
    [MinMaxSlider(0, 100, true)] [SerializeField] private Vector2Int _explosionDamage;
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private ParticleSystem _explosionEffect;

    private Arrow _arrow;

    private void Awake()
    {
        _arrow = GetComponent<Arrow>();
        _arrow.OnImpact += OnHit;
    }

    private void OnHit(ArrowHit hit)
    {
        Explode(hit);
    }

    private void Explode(ArrowHit hit)
    {
        Vector3 hitPoint = hit.HitPoint;
        Collider[] colliders = Physics.OverlapSphere(hitPoint, _explosionRadius, _collisionLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(_explosionForce, hitPoint, _explosionRadius);
            }

            if (collider.TryGetComponent(out IDamageable damageable))
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = CalculateDamageBasedOnDistanceAndAccuracy(hitPoint, collider.transform.position, _arrow.GetAccuracy());
                damageInfo.damagePosition = collider.ClosestPoint(transform.position);
                damageInfo.hitDirection = (collider.transform.position - transform.position).normalized;
                damageable.Damage(damageInfo);
            }
        }

        ObjectPoolManager.SpawnObject(_explosionEffect, hitPoint, Quaternion.identity);
    }

    private int CalculateDamageBasedOnDistanceAndAccuracy(Vector3 hitPoint, Vector3 targetPoint, float accuracy)
    {
        float distance = Vector3.Distance(hitPoint, targetPoint);
        float damage = _explosionDamage.y - (_explosionDamage.y - _explosionDamage.x) * (distance / _explosionRadius);
        damage *= accuracy;
        return Mathf.RoundToInt(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
