using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private CheckpointTrigger[] _checkpoints;
    private CheckpointTrigger lastCheckPoint;

    private void Awake()
    {
        Instance = this;
        _checkpoints = GetComponentsInChildren<CheckpointTrigger>();
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
}
