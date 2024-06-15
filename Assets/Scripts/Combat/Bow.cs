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
    [SerializeField] private Transform _arrow;
    [SerializeField] private Transform _arrowEffect;
    [SerializeField] private ParticleSystem _chargeEffect;

    [Header("Effect")]
    [SerializeField] private Vector3 _scaleEffectSize = Vector3.one * 1.25f;
    [SerializeField] private float _scaleEffectDuration = 0.15f;
    [SerializeField] private Ease _ease;
    [SerializeField] private float minArrowShakeIntensity = 0.1f;
    [SerializeField] private float maxArrowShakeIntensity = 1f;
    [SerializeField] private Gradient _arrowShakeGradient;

    [Header("SO Settings")]

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _nextFireTime;
    [SerializeField] private float _damage;
    [SerializeField] private LayerMask _targetLayer;

    private State state = State.Normal;
    private float normalisedAccuracy;
    private Vector3 _arrowLocalPosition;
    private JuicerRuntimeCore<Vector3> _scaleEffect;

    public Transform GetFirePoint () => _firePoint;

    private void Awake()
    {
        _scaleEffect = _visual.transform.JuicyScale(_scaleEffectSize, _scaleEffectDuration);
        _scaleEffect.SetLoop(2).SetEase(_ease);
        _arrowLocalPosition = _arrow.localPosition;
    }

    private void Update()
    {
        if  (state == State.Pulling)
        {
            float intensity = Mathf.Lerp(minArrowShakeIntensity, maxArrowShakeIntensity, normalisedAccuracy);
            _arrow.localPosition = _arrowLocalPosition + Random.insideUnitSphere * intensity;
            _arrowEffect.localScale = Vector3.one * Mathf.Lerp(0, 1f, normalisedAccuracy);
            SetChargeParticleColor(_arrowShakeGradient.Evaluate(normalisedAccuracy));
        }
        else
        {
            _arrow.localPosition = _arrowLocalPosition;
        }
    }

    public void FireProjectile (Vector3 direction)
    {

    }

    public void SetState (State state)
    {
        this.state = state;
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

    private void SetChargeParticleColor (Color color)
    {
        var main = _chargeEffect.main;
        main.startColor = color;
    }

    public void SetAccuracy (float accuracy)
    {
        normalisedAccuracy = accuracy;
    }

    public void OnFire()
    {
        _scaleEffect.Start();
    }
}
