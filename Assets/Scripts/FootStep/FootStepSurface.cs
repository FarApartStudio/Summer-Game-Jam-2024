using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FootSurface
{
    Dirt,
    Grass,
    Wood,
    Rock,
    Water
}

public class FootStepSurface : MonoBehaviour
{
    [ SerializeField] private FootSurface surface;

    public FootSurface GetFootSurface => surface;
}
