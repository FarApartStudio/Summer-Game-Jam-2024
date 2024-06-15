using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.SurfaceSystem
{
    public enum SurfaceType
    {
        Metall, Rock, Wood, Concrete, Dirt, Blood
    }

    [System.Serializable]
    public class SurfaceInfo
    {
        public SurfaceType _surfaceType;
        public GameObject _particleSystem;
    }
}

