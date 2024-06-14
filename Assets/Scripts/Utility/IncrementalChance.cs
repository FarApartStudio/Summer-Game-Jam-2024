using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IncrementalChance
{
    [Range(0, 1)]
    [SerializeField] float _baseChance = 0.2f;
    [Range(0, 1)]
    [SerializeField] float _missIncrement = 0.1f;

    [Header("Debug")]
    [SerializeField] float _currentChance;

    public bool ChanceHit()
    {
        bool hit = _currentChance >= Random.value;
        if (hit)
        {
            _currentChance = _baseChance;
        }
        else
        {
            _currentChance = _baseChance + _missIncrement;
        }
        return hit;
    }
}
