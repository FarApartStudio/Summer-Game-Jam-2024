using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    public Action<EnemyController> OnActivated;
    public Action<EnemyController> OnKilled;

    [SerializeField] private List<EnemyController> enemies;

    public void Init()
    {
        foreach (var enemy in enemies)
        {
            enemy.Activate();
            enemy.OnKilled += EnemyKilled;
            OnActivated?.Invoke(enemy);
        }
    }

    private void EnemyKilled(EnemyController controller)
    {
        controller.OnKilled -= EnemyKilled;
        OnKilled?.Invoke(controller);
    }
}
