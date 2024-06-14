using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHanging : MonoBehaviour
{
    [Header("Movement Effect")]
    [SerializeField] private Vector3 _moveDistance = new Vector3(0, 10f, 0);
    [SerializeField] private float _movementDuration = 0.5f;
    [SerializeField] Ease _movementEaseType = Ease.EaseInOutBack;

    private JuicerRuntimeCore<Vector3> _movementEffect;

    private void OnEnable()
    {
        if (_movementEffect == null)
        {
            _movementEffect = transform.JuicyMove(transform.position + _moveDistance, _movementDuration);
            _movementEffect.SetLoop(0).SetEase(_movementEaseType);
        }

        _movementEffect.Start();
    }

    private void OnDisable()
    {
        if (_movementEffect != null)
        {
            _movementEffect.Pause();
        }
    }

    public void ToggleActive(bool state)
    {
        if (state)
        {
            _movementEffect.Start();
        }
        else
        {
            _movementEffect.Pause();
        }
    }
}
