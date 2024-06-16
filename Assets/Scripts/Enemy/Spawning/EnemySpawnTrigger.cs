using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{
    public event EventHandler OnSpawnTriggered;
    public event EventHandler OnAllEnemiesKilled;

    [SerializeField] private float activationDelay;
    [SerializeField] private EnemySpawnArea[] enemySpawnAreaArray;
    [SerializeField] private ZoneBlocker[] zoneBlockerArray;

    [Header("Debug")]
    private List<EnemySpawnArea> activeSpawnAreaList;
    private bool alreadyTriggered;

    private void Awake()
    {
        activeSpawnAreaList =  new List<EnemySpawnArea>();
    }

    private void OnEnable()
    {
        foreach (var spawnArea in enemySpawnAreaArray)
        {
            spawnArea.OnCleared += Area_OnCleared;
        }
    }

    private void Area_OnCleared(object sender, EventArgs e)
    {
        if(sender is EnemySpawnArea spawnArea)
        {
            if (activeSpawnAreaList.Contains(spawnArea)) activeSpawnAreaList.Remove(spawnArea);
        }

        CheckIfAllEnemiesCleared();
    }

    public void CheckIfAllEnemiesCleared()
    {
        if (activeSpawnAreaList.Count == 0)
        {
            OnAllEnemiesKilled?.Invoke(this, EventArgs.Empty);

            ToggleZoneBlockers(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered) return;

        //if (other.gameObject == BaseHero.PlayerHero.gameObject)
        //{
        //    alreadyTriggered = true;

        //    OnSpawnTriggered?.Invoke(this, EventArgs.Empty);

        //    StartCoroutine(TriggerRoutine());
        //}
    }

    IEnumerator TriggerRoutine()
    {
        ToggleZoneBlockers(true);

        yield return new WaitForSeconds(activationDelay);

        // Spawn areas
        foreach (var spawn in enemySpawnAreaArray)
        {
            activeSpawnAreaList.Add(spawn);
            spawn.StartSpawn();
        }
    }

    public void ToggleZoneBlockers(bool enable)
    {
        //foreach (var zoneBlocker in zoneBlockerArray)
        //{
        //    if(enable)
        //    {
        //        zoneBlocker.EnableWall();
        //    }
        //    else
        //    {
        //        zoneBlocker.DisableWall();
        //    }
        //}
    }

    private void OnDisable()
    {
        foreach (var spawnArea in enemySpawnAreaArray)
        {
            spawnArea.OnCleared -= Area_OnCleared;
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
