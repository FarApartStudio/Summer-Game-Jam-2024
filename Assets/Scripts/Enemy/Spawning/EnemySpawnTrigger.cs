using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnTrigger : MonoBehaviour
{
    public Action<EnemyController> OnActivated;
    public Action<EnemyController> OnKilled;

    [SerializeField] private int maxEnemiesAive = 4;
    [SerializeField] private float activationDelay;


    [SerializeField] private UnityEvent OnTriggered;
    [SerializeField] private UnityEvent OnClear;

    private EnemySpawnArea[] enemySpawnAreaArray;

    [Header("Debug")]
    private bool alreadyTriggered;

    public void Init()
    {
        enemySpawnAreaArray = GetComponentsInChildren<EnemySpawnArea>();

        foreach (var spawnArea in enemySpawnAreaArray)
        {
            spawnArea.CanSpawnEnemies = CanSpawnEnemies;
            spawnArea.OnActivated = OnActivated;
            spawnArea.OnKilled = OnKilled;
            spawnArea.OnCleared = OnCleared;
        }
    }


    private void OnCleared()
    {
        CheckIfAllEnemiesCleared();
    }

    public void CheckIfAllEnemiesCleared()
    {
        foreach (var enemySpawnArea in enemySpawnAreaArray)
        {
            if (!enemySpawnArea.IsCleared)
            {
                return;
            }
        }

        OnClear?.Invoke();
    }

    public bool CanSpawnEnemies()
    {
        int enemiesAlive = 0;

        foreach (var spawnArea in enemySpawnAreaArray)
        {
            enemiesAlive += spawnArea.EnemiesCount;
        }

        return enemiesAlive < maxEnemiesAive;
    }

    void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;

        if (other.TryGetComponent(out Pilot pilot))
        {
            alreadyTriggered = true;
            StartCoroutine(TriggerRoutine());
        }
    }

    IEnumerator TriggerRoutine()
    {
        OnTriggered?.Invoke();

        yield return new WaitForSeconds(activationDelay);

        foreach (var spawn in enemySpawnAreaArray)
        {
            spawn.StartSpawn();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        Gizmos.color = new Color(0.5f, 1, 0.5f, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(0, 1, 0, 0.7f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
