using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.SurfaceSystem
{
    public class Surface : MonoBehaviour
    {
        [SerializeField] private SurfaceType _surfaceType = SurfaceType.Metall;
        [Range(0, 1)]
        [SerializeField] private float _friction = 0.5f;

        public SurfaceType GetSurfaceType { get => _surfaceType; }
        public float GetFriction { get => _friction; }

        public SurfaceInfo GetSurfaceInfo()
        {
            return SurfaceManager.Instance.GetSurfaceInfo(_surfaceType);
        }
    }
}

