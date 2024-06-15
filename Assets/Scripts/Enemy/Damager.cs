using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

public class Damager : MonoBehaviour
{
	[Header("Damage")]
    [SerializeField] protected DamageInfo _damageInfo;
    [SerializeField] protected int _criticalDamage;
	[SerializeField] protected float _criticalChance;
	[SerializeField] protected LayerMask _targetLayer;
	[SerializeField] protected bool _hasHit;

	public MonoBehaviour owner { get; private set; }

	public Action OnHit;

	public DamageInfo damageInfo => _damageInfo;

	public void SetUp(MonoBehaviour owner,int currentCriticalDamage, float currentCriticalChance)
    {
		this.owner = owner;
        _criticalDamage = currentCriticalDamage;
		_criticalChance = currentCriticalChance;
	}

	public virtual void Attack(){}

	protected int RandomCriticalDamage()
	{
		if ((_criticalChance / 100f) >= Random.value) return _criticalDamage;
		else return 0;
	}

	public bool HasHit() { return _hasHit; }
}

public enum DamageType { Melee, Projectile }

[Serializable]
public class DamageInfo
{
    public DamageType damageType;

    public int damage;
    public bool critical;
    public bool knockback;
    public bool stun;
    public Vector3 HitDirection;
	public float damageMultiply = 1;
    public int damageIncrease;
    public int damageIncreasePercent;
}