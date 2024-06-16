using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class DamageDefector : MonoBehaviour
{
    public Action OnDefect;

    [SerializeField] private float _defectChance = 0.5f;
    private Vector3 _lastDefectPosition;

    public Vector3 GetLastDefectPosition()
    {
        return _lastDefectPosition;
    }
    public bool Defect(Vector3 position)
    {
        float randomValue = Random.value;
        bool canDefect = randomValue > _defectChance;
        if (canDefect)
        {
            _lastDefectPosition = position;
            OnDefect?.Invoke();
        }
        return canDefect;
    }
}
