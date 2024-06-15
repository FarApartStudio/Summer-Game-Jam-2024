using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public enum State { Normal, Pulling };

    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _normalString;
    [SerializeField] private GameObject _pullString;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _nextFireTime;
    [SerializeField] private float _damage;
    [SerializeField] private LayerMask _targetLayer;


    public Transform GetFirePoint () => _firePoint;


    public void FireProjectile (Vector3 direction)
    {

    }

    public void SetState (State state)
    {
        switch (state)
        {
            case State.Normal:
                _normalString.SetActive(true);
                _pullString.SetActive(false);
                break;
            case State.Pulling:
                _normalString.SetActive(false);
                _pullString.SetActive(true);
                break;
        }
    }
}
