using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pelumi.ObjectPool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect;
    private LayerMask _layer;
    private Vector3 _m_direction;
    private float _moveSpeed = 0;
    private RangedDamager _damager;
    private DamageInfo _damageInfo = new DamageInfo();


    private void Update()
    {
        transform.Translate(_m_direction * _moveSpeed * Time.deltaTime);
    }

    public void SetUp(RangedDamager currentDamager, LayerMask targetLayer, Vector3 _direction, float _speed, DamageInfo damageInfo)
    {
        _damager = currentDamager;
        _layer = targetLayer;
        _damageInfo = damageInfo;
        _m_direction = _direction;
        _moveSpeed = _speed;
    }

    public void SetUp(RangedDamager currentDamager, LayerMask targetLayer, DamageInfo damageInfo)
    {
        _damager = currentDamager;
        _layer = targetLayer;
        _damageInfo = damageInfo;
    }

    private void OnTriggerEnter(Collider other)
    {
       bool hasHit = true;

        if (other.transform.CompareTag("Wall")) SpawnHitEffect(transform);

        if ((_layer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                _damageInfo.damagePosition = transform.position;
                _damageInfo.hitDirection = _m_direction.normalized;
                hasHit = damageable.Damage(_damageInfo);
                SpawnHitEffect(transform);
            }
        }
        else hasHit = false;

        if (hasHit) _damager.OnHit?.Invoke();
    }

    public void SpawnHitEffect(Transform spawnPos)
    {
        ObjectPoolManager.SpawnObject(hitEffect.gameObject, spawnPos.position, Quaternion.Euler(0, 0, 0));
        StartCoroutine(DisableObjectDelayed(0.1f));
    }


    private IEnumerator DisableObjectDelayed(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        ObjectPoolManager.ReleaseObject(gameObject);
    }
}
