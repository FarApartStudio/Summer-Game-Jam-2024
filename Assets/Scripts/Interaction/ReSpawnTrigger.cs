using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawnTrigger : MonoBehaviour
{
    private Transform[] spawnPoints;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            pilot.gameObject.SetActive(false);  
            pilot.transform.position = spawnPoint.position;
            pilot.transform.rotation = spawnPoint.rotation;
            pilot.gameObject.SetActive(true);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
