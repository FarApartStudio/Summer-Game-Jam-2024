using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    public event EventHandler OnCleared;

    [Header("Propeties")]
    [SerializeField] float timeBetweenWaves;
    [Range(0.0f, 100.0f)]
    [SerializeField] float chanceToAttackMainHero;
    [SerializeField] Wave[] waveArray;

    [Header("Debug")]
    private List<EnemyController> enemiesList;

    private void Awake()
    {
        enemiesList = new List<EnemyController>();
    }

    private void OnEnable()
    {

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
                    //Enemy newEnemy = EnemyPool.Get(waveArray[i].waveEnemyArray[j].enemySO);
                    //newEnemy.gameObject.transform.position = GetRandomSpawnPoint();
                    //newEnemy.gameObject.transform.rotation = Quaternion.identity;

                    //BaseHero target = GetTargetHero();

                    //enemiesList.Add(newEnemy);

                    //newEnemy.transform.LookAt(target.transform);

                    //newEnemy.SetTarget(target);

                    //newEnemy.gameObject.SetActive(true);

                    //yield return null;
                    //Effects.EnemySpawnFX(newEnemy.transform.position);
                }
            }

            // Delay until next wave

            if(i < waveArray.Length - 1)
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        FinishedSpawning();
    }

    private void FinishedSpawning()
    {
        Debug.Log("Finished Spawning");
    }

 
    public Vector3 GetRandomSpawnPoint()
    {
        Vector3 randomPosition =  new Vector3(
                    UnityEngine.Random.Range(GetComponent<BoxCollider>().bounds.min.x, GetComponent<BoxCollider>().bounds.max.x),
                    UnityEngine.Random.Range(GetComponent<BoxCollider>().bounds.min.y, GetComponent<BoxCollider>().bounds.max.y),
                    UnityEngine.Random.Range(GetComponent<BoxCollider>().bounds.min.z, GetComponent<BoxCollider>().bounds.max.z));
        randomPosition.y = 6.006234f;
        return randomPosition;
    }

    private void OnDisable()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}

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
