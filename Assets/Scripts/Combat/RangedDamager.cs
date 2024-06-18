using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pelumi.ObjectPool;

public class RangedDamager : Damager
{
	[Header("Set Up")]
	[SerializeField] private int numberOfProj = 1;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private LayerMask _targetLayer;

    private Transform target;
    private Vector3 targetPosition;
    private Vector3 direction;
    private int oddProjectile;
	private int evenProjectile;

    public override void Attack()
	{
        int currentCriticalDamage = RandomCriticalDamage();

        DamageInfo _damageInfo = new DamageInfo();
        _damageInfo.damage = _damageInfo.damage + currentCriticalDamage;
        _damageInfo.critical = currentCriticalDamage > 0;

        direction = GetPlayerDirection();
        targetPosition = target.position;

        evenProjectile = 10;
		oddProjectile = -10;

		for (int i = 0; i < numberOfProj; i++)
        {
            Projectile projectile = ObjectPoolManager.SpawnObject(projectilePrefab, transform.position, Quaternion.identity);
            projectile.SetUp(this, _targetLayer, direction, projectileSpeed, _damageInfo);

            if (i % 2 == 0) //Even
            {
                direction = Quaternion.Euler(0, evenProjectile, 0) * GetPlayerDirection();
                evenProjectile += evenProjectile;
            }
            else //Odd
            {
                direction = Quaternion.Euler(0, oddProjectile, 0) * GetPlayerDirection();
                oddProjectile += oddProjectile;
            }
        }
	}

	public Vector3 GetPlayerDirection()
    {
		return (target.position - transform.position).normalized;
	}
}
