using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private CheckpointTrigger[] _checkpoints;
    private CheckpointTrigger lastCheckPoint;

    private void Awake()
    {
        Instance = this;

        if (_checkpoints == null || _checkpoints.Length == 0)
        {
            GenerateAllACheckpoints();
        }
    }

    public CheckpointTrigger GetActiveCheckpoint()
    {
        return lastCheckPoint;
    }

    public void ResetCheckpoint()
    {
        lastCheckPoint = _checkpoints[0];
    }

    public void RegisterCheckpoint(CheckpointTrigger checkpointTrigger)
    {
        lastCheckPoint = checkpointTrigger;
    }

    [Button]
    private void GenerateAllACheckpoints()
    {
        _checkpoints = GetComponentsInChildren<CheckpointTrigger>();
    }
}
