using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;
using static AttackInfo;

public abstract class Damager : MonoBehaviour
{
	[Header("Damage")]
    [SerializeField] protected int _criticalDamage;
	[SerializeField] protected float _criticalChance;
	[SerializeField] protected bool _hasHit;

    protected DamageInfo _damageInfo;

	public MonoBehaviour owner { get; private set; }

	public Action OnHit;

	public void SetUp(MonoBehaviour owner,int currentCriticalDamage, float currentCriticalChance)
    {
		this.owner = owner;
        _criticalDamage = currentCriticalDamage;
		_criticalChance = currentCriticalChance;
	}

	public void SetDamageInfo (DamageInfo damageInfo)
	{
        _damageInfo = damageInfo;
    }

	public abstract void Attack();

	protected int RandomCriticalDamage()
	{
		if ((_criticalChance / 100f) >= Random.value) return _criticalDamage;
		else return 0;
	}

	public bool HasHit() { return _hasHit; }
}

public enum DamageType { Melee, Projectile }

[Serializable]
public struct DamageInfo
{
    public AttackType attackType;
    public DamageType damageType;
    public int damage;
    public bool critical;
    public bool knockback;
    public bool stun;
	public Vector3 damagePosition;
    public Vector3 hitDirection;
}