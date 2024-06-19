using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    [SerializeField] private EnemyController _enemyController;

    public HealthController GetHealthController => _enemyController.healthController;    


    public void Activate (Transform target)
    {
        gameObject.SetActive (true);
        _enemyController.Activate(false);
        _enemyController.SetTarget(target);
    }
}
