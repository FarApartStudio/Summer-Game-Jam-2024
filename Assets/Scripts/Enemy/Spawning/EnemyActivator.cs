using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyActivator : MonoBehaviour
{
    public Action<EnemyController> OnActivated;
    public Action<EnemyController> OnKilled;

    [SerializeField] private EnemyController[] enemies;
    [SerializeField] private UnityEvent OnClear;

    [Button]
    private void GenerateEnemies()
    {
        enemies  = GetComponentsInChildren<EnemyController>();
    }

    public void Init()
    {
        if (enemies == null || enemies.Length == 0)
        {
            GenerateEnemies();
        }

        foreach (var enemy in enemies)
        {
            enemy.Activate(true);
            enemy.OnKilled += EnemyKilled;
            OnActivated?.Invoke(enemy);
        }
    }

    private void EnemyKilled(EnemyController controller)
    {
        controller.OnKilled -= EnemyKilled;
        OnKilled?.Invoke(controller);

        if (IsAllEnemiesDead())
        {
            OnClear?.Invoke();
        }
    }

    private bool IsAllEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead)
            {
                return false;
            }
        }

        return true;
    }
}
