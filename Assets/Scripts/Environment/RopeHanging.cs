using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RopeHanging : MonoBehaviour
{
    [Header("Movement Effect")]
    [MinMaxSlider(0, 10, true)]
    [SerializeField] private Vector2 _startDelay = new Vector2(0, 0);
    [SerializeField] private Vector3 _moveDistance = new Vector3(0, 10f, 0);
    [SerializeField] private float _movementDuration = 0.5f;
    [SerializeField] Ease _movementEaseType = Ease.EaseInOutBack;

    private JuicerRuntimeCore<Vector3> _movementEffect;

    private void OnEnable()
    {
        if (_movementEffect == null)
        {
            _movementEffect = transform.JuicyMove(transform.position + _moveDistance, _movementDuration);
            _movementEffect.SetLoop(0).SetEase(_movementEaseType).SetDelay(Random.Range(_startDelay.x, _startDelay.y));
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
