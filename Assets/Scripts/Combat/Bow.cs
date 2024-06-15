using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public enum State { Normal, Pulling };

    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _normalString;
    [SerializeField] private GameObject _pullString;
    [SerializeField] private GameObject _visual;

    [Header("Effect")]
    [SerializeField] private Vector3 _scaleEffectSize = Vector3.one * 1.25f;
    [SerializeField] private float _scaleEffectDuration = 0.15f;
    [SerializeField] private Ease _ease;

    [Header("SO Settings")]

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _nextFireTime;
    [SerializeField] private float _damage;
    [SerializeField] private LayerMask _targetLayer;

    private JuicerRuntimeCore<Vector3> _scaleEffect;

    public Transform GetFirePoint () => _firePoint;

    private void Awake()
    {
        _scaleEffect = _visual.transform.JuicyScale(_scaleEffectSize, _scaleEffectDuration);
        _scaleEffect.SetLoop(2).SetEase(_ease);
    }

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

    public void OnFire()
    {
        _scaleEffect.Start();
    }
}
