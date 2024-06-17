using Cinemachine.Utility;
using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Pelumi.Juicer;
using Pelumi.ObjectPool;

public struct DamageData
{
    public IHealth instigator;
    public int damage;
    public bool critical;
    public DamageType damageType;
}

public interface IHealth
{
    Transform transform { get; }
    void DealDamage(DamageInfo healthModification);
    void RestoreHeal(int amount);
    int GetCurrentHealth { get; }
    int GetMaxHealth { get; }
    float GetNormalisedHealth { get; }
    bool IsInvulnerable { get; }

    bool IsAlive { get; }
    void SetInvisibility(bool status);

    void AddDamageEffect(int damage, float damageInterval, float duration,GameObject effectPrefab = null, Action OnEffectEnd = null);
}

public interface IBreakable
{
    void Break();
}

public class HealthController : MonoBehaviour, IHealth
{
    public static event Action<HealthController> OnSetUp;
    public static event Action<HealthController> OnDespawn;

    public event Action<IHealth> OnHealthChanged;
    public event Action<DamageInfo> OnHit;
    public event Action<int> OnHeal;
    public event Action<DamageInfo> OnDie;
    public event Action<DamageInfo> OnHitWhileInvisible;

    [BoxGroup("Health")] [SerializeField] private int currentHealth;
    [BoxGroup("Health")] [SerializeField] private int maxHealth;
    [BoxGroup("Health")] [SerializeField] private float damageDelayDuration;

    [BoxGroup("Debug")] [SerializeField] private bool isInvisible = false;
    [BoxGroup("Debug")] [SerializeField] private bool canDealDamage = true;
    [BoxGroup("Debug")] [SerializeField] private List<DamageEffect> _damageEffects = new List<DamageEffect>();

    WaitForSeconds damageDelay;

    public void SetInvisibility(bool status) => isInvisible = status;
    public int GetCurrentHealth => currentHealth;
    public int GetMaxHealth => maxHealth;
    public float GetNormalisedHealth => (float)currentHealth / maxHealth;
    public bool IsInvulnerable => isInvisible;

    public bool HasDamageDelay => damageDelayDuration > 0;

    public bool IsAlive => currentHealth > 0;

    private void Awake()
    {
        if (HasDamageDelay)
        damageDelay = new WaitForSeconds(damageDelayDuration);
    }

    private void Start()
    {
        // to remove 
        OnSetUp?.Invoke(this);
    }

    public void SetUp(int maxHealth)
    {
        StopDamageEffects();
        SetMaxHealth(maxHealth);
        OnSetUp?.Invoke(this);
    }

    public void SetMaxHealth(int amount)
    {
        maxHealth = currentHealth = amount;
        OnHealthChanged?.Invoke(this);
    }

    public void DealDamage(DamageInfo damageInfo)
    {
        if (!canDealDamage || !IsAlive)
            return; 

        if (isInvisible)
        {
            OnHitWhileInvisible?.Invoke(damageInfo);
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageInfo.damage, 0, maxHealth);

        OnHealthChanged?.Invoke(this);

        if (currentHealth > 0)
        {
            OnHit?.Invoke(damageInfo);

            if (HasDamageDelay)
                StartCoroutine(ResetDamageDelay());
        }
        else
        {
            OnDie?.Invoke(damageInfo);
            OnDespawn?.Invoke(this);
        }
    }

    public void RestoreHeal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHeal?.Invoke(amount);
        OnHealthChanged?.Invoke(this);
    }

    public IEnumerator ResetDamageDelay()
    {
        canDealDamage = false;
        yield return damageDelay;
        canDealDamage = true;
    }

    public void AddDamageEffect(int damage, float damageInterval, float duration, GameObject effectPrefab = null,Action OnEffectEnd = null)
    {
        DamageEffect damageEffect = _damageEffects.Find(x => !x.IsActive);
        if (damageEffect == null)
        {
            damageEffect = new DamageEffect(this, this, damage, damageInterval, duration, effectPrefab);
            _damageEffects.Add(damageEffect);
        }
        else
        {
            damageEffect.Activate(this, this, damage, damageInterval, duration,effectPrefab);
        }
        damageEffect.OnEffectEnd = OnEffectEnd;
    }

    public void StopDamageEffects()
    {
        foreach (var effect in _damageEffects)
        {
            effect.RemoveEffect();
        }
    }
}

[Serializable]
public class DamageEffect
{
    public Action OnEffectEnd;
    private float _duration;

    [SerializeField] private bool _isActive;
    [SerializeField] private int _damage;
    [SerializeField] private float _damageInterval;
    [SerializeField] private GameObject _effectVisual;

    private Coroutine _effectCoroutine;
    private MonoBehaviour _instigator;
    private IHealth _health;

    public bool IsActive => _isActive;

    public DamageEffect(MonoBehaviour instigator, IHealth health, int damage, float damageInterval, float duration, GameObject _effectPrefab = null)
    {
        Activate(instigator, health, damage, damageInterval, duration, _effectPrefab);
    }

    public void Activate(MonoBehaviour instigator, IHealth health, int damage, float damageInterval, float duration, GameObject _effectPrefab = null)
    {
        _instigator = instigator;
        _duration = duration;
        _isActive = true;
        _health = health;
        _damage = damage;
        _damageInterval = damageInterval;
        _effectCoroutine = instigator.StartCoroutine(EffectRoutine());
        if (_effectPrefab != null)
            _effectVisual = ObjectPoolManager.SpawnObject(_effectPrefab, health.transform);
    }

    IEnumerator EffectRoutine()
    {
        while (_duration > 0)
        {
            _health.DealDamage(new DamageInfo { damage = _damage });
            yield return new WaitForSeconds(_damageInterval);
            _duration -= _damageInterval;
        }
        ClearEffect();
        OnEffectEnd?.Invoke();
    }

    public void RemoveEffect()
    {
        if (!_isActive)
            return;

        _instigator.StopCoroutine(_effectCoroutine);
        ClearEffect();
    }

    private void ClearEffect()
    {
        _isActive = false;

        if (_effectVisual != null)
        {
            ObjectPoolManager.ReleaseObject(_effectVisual);
            _effectVisual = null;
        }
    }
}