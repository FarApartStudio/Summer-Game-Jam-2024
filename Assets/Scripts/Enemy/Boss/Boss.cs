using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;

    private void Start()
    {
        Activate();
    }

    public void Activate ()
    {
        _enemyController.Activate(false);
    }

    public void SetTarget (Transform target)
    {
        _enemyController.SetTarget(target);
    }
}
