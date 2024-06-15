using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class DamageDefector : MonoBehaviour
{
    public Action OnDefect;

    [MinMaxSlider(0, 10, true)] [SerializeField] private Vector2 _defectActiveTimeRange;
    private Vector3 _lastDefectPosition;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(ResetTimer());
    }

    public Vector3 GetLastDefectPosition()
    {
        return _lastDefectPosition;
    }
    public void Defect(Vector3 position)
    {
        _lastDefectPosition = position;
        OnDefect?.Invoke();
        StartCoroutine(ResetTimer());
    }

    public IEnumerator ResetTimer()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(Random.Range(_defectActiveTimeRange.x, _defectActiveTimeRange.y));
        _collider.enabled = true;
    }
}
