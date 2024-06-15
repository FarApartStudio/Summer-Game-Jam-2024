using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.SurfaceSystem
{
    public class SurfaceManager : MonoBehaviour
    {
        public static SurfaceManager Instance { get; private set; }

        [SerializeField] private SurfaceInfo[] _surfaceInfos;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public SurfaceInfo GetSurfaceInfo(SurfaceType surfaceType)
        {
            foreach (var surfaceInfo in _surfaceInfos)
            {
                if (surfaceInfo._surfaceType == surfaceType)
                {
                    return surfaceInfo;
                }
            }
            return null;
        }
    }
}

