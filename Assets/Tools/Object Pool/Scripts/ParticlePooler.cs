using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePooler : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
       ObjectPoolManager.ReleaseObject(this);
    }
}
