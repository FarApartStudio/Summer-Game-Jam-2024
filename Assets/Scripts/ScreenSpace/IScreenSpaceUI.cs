using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IScreenSpaceUI
{
    public Action<float> GetDistanceFromCamera { get; }
    public bool IsActive { get; }
    public Transform transfrom { get; }
    public Transform Target { get; }
    public float YOffset { get; }
    public void Spawn(Transform target);
    public void ReturnToPool();
}
