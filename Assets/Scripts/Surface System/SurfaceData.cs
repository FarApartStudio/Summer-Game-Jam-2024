using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.SurfaceSystem
{
    public enum SurfaceType
    {
        Metal, Rock, Wood,Enemy
    }

    [System.Serializable]
    public class SurfaceInfo
    {
        public SurfaceType _surfaceType;
        public GameObject _particleSystem;
        public AudioClip _audioClip;
    }
}

