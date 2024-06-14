using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIndicator : MonoBehaviour
{
    [Header("Movement Effect")]
    [SerializeField] private Vector3 _moveDistance = new Vector3(0, 10f,0);
    [SerializeField] private float _movementDuration = 0.5f;

    [Header("Scale Effect")]
    [SerializeField] private Vector3 _maxScale = Vector3.one * 1.5f;
    [SerializeField] private float _scaleDuration = 0.5f;

    private JuicerRuntimeCore<Vector3> _movementEffect;
    private JuicerRuntimeCore<Vector3> _scaleEffect;

    private void OnEnable()
    {
        if (_movementEffect == null)
        {
            _movementEffect = transform.JuicyMove(transform.position + _moveDistance, _movementDuration);
            _movementEffect.SetLoop(0);
        }

        if (_scaleEffect == null)
        {
            _scaleEffect = transform.JuicyScale(_maxScale, _scaleDuration);
            _scaleEffect.SetLoop(0);
        }

        _scaleEffect.Start();

        _movementEffect.Start();
    }

    private void OnDisable()
    {
        if (_movementEffect != null)
        {
            _movementEffect.Pause();
        }

        if (_scaleEffect != null)
        {
            _scaleEffect.Pause();
        }
    }

    public void ToggleActive(bool state)
    {
        if (state)
        {
            _movementEffect.Start();
            _scaleEffect.Start();
        }
        else
        {
            _movementEffect.Pause();
            _scaleEffect.Pause();
        }
    }
}
