using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyActivator : MonoBehaviour
{
    public Action<EnemyController> OnActivated;
    public Action<EnemyController> OnKilled;

    [SerializeField] private List<EnemyController> enemies;
    [SerializeField] private UnityEvent OnClear;

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
