using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public WaveEnemy[] waveEnemyArray;
}

[System.Serializable]
public struct WaveEnemy
{
    public EnemyData enemySO;
    public int amount;
}

public class EnemySpawnArea : MonoBehaviour
{
    public Func<bool> CanSpawnEnemies;

    public Action<EnemyController> OnActivated;
    public Action<EnemyController> OnKilled;
    public Action OnCleared;

    [Header("Propeties")]
    [SerializeField] float timeBetweenWaves;
    [SerializeField] Wave[] waveArray;

    [Header("Debug")]
    private List<EnemyController> enemiesList = new List<EnemyController>();
    private BoxCollider boxCollider;

    public bool IsCleared => enemiesList.Count == 0;
    public int EnemiesCount => enemiesList.Count;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void StartSpawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < waveArray.Length; i++)
        {
            for (int j = 0; j < waveArray[i].waveEnemyArray.Length; j++)
            {
                for (int k = 0; k < waveArray[i].waveEnemyArray[j].amount; k++)
                {
                    yield return new WaitUntil(CanSpawnEnemies);

                    EnemyController newEnemy = ObjectPoolManager.SpawnObject(waveArray[i].waveEnemyArray[j].enemySO.enemyController, transform.position,
                        Quaternion.identity);
                    newEnemy.gameObject.SetActive(false);
                    newEnemy.OnKilled += EnemyKilled;
                    enemiesList.Add(newEnemy);
                    OnActivated?.Invoke(newEnemy);
                    newEnemy.gameObject.SetActive(true);
                    newEnemy.Activate(false);
                    yield return null;
                }
            }

            yield return new WaitUntil(() => IsCleared);

            if(i < waveArray.Length - 1)
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        FinishedSpawning();
    }

    private void EnemyKilled(EnemyController controller)
    {
        controller.OnKilled -= EnemyKilled;
        OnKilled?.Invoke(controller);
        enemiesList.Remove(controller);

        if (IsCleared)
        {
            OnCleared?.Invoke();
        }
    }

    private void FinishedSpawning()
    {

    }

    public Vector3 GetRandomSpawnPoint()
    {
        Vector3 randomPosition =  new Vector3(
                    UnityEngine.Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x),
                    UnityEngine.Random.Range(boxCollider.bounds.min.y, boxCollider.bounds.max.y),
                    UnityEngine.Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z));
        randomPosition.y = 6.006234f;
        return randomPosition;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        //Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        //Gizmos.DrawCube(Vector3.zero, Vector3.one);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}