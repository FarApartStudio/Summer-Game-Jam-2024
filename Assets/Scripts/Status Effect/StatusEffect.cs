using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    //public Action OnEffectEnd;
    //private float _duration;
    //[SerializeField] private bool _isActive;
    //private Coroutine _effectCoroutine;
    //private MonoBehaviour _instigator;
    //private IHealth _health;
    //private int _damage;
    //private float _damageInterval;
    //private GameObject _effectVisual;

    //public bool IsActive => _isActive;

    //public DamageEffect(MonoBehaviour instigator, IHealth health, int damage, float damageInterval, float duration, GameObject _effectPrefab = null)
    //{
    //    Activate(instigator, health, damage, damageInterval, duration, _effectPrefab);
    //}

    //public void Activate(MonoBehaviour instigator, IHealth health, int damage, float damageInterval, float duration, GameObject _effectPrefab = null)
    //{
    //    _instigator = instigator;
    //    _duration = duration;
    //    _isActive = true;
    //    _health = health;
    //    _damage = damage;
    //    _damageInterval = damageInterval;
    //    _effectCoroutine = instigator.StartCoroutine(EffectRoutine());
    //    if (_effectPrefab != null)
    //        _effectVisual = GameObject.Instantiate(_effectPrefab, health.transform);
    //}

    //IEnumerator EffectRoutine()
    //{
    //    while (_duration > 0)
    //    {
    //        _health.DealDamage(new DamageInfo { damage = _damage });
    //        yield return new WaitForSeconds(_damageInterval);
    //        _duration -= _damageInterval;
    //    }
    //    OnEffectEnd?.Invoke();
    //    ClearEffect();
    //}

    //public void RemoveEffect()
    //{
    //    if (!_isActive)
    //        return;
    //    _instigator.StopCoroutine(_effectCoroutine);
    //    ClearEffect();
    //}

    //private void ClearEffect()
    //{
    //    _isActive = false;
    //    if (_effectVisual != null)
    //        GameObject.Destroy(_effectVisual);
    //}
}
