using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private Transform _spawnPos;

    public Transform GetSpawnPos => _spawnPos;

    public void RegisterAsLastCheckPoint ()
    {
        CheckpointManager.Instance.RegisterCheckpoint(this);
    }
}
