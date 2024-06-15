using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDefector : MonoBehaviour
{
    public Action OnDefect;

    private Vector3 _lastDefectPosition;

    public Vector3 GetLastDefectPosition()
    {
        return _lastDefectPosition;
    }
    public void Defect(Vector3 position)
    {
        _lastDefectPosition = position;
        OnDefect?.Invoke();
    }
}
