using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    Health,
    Ammo,
    Rune
}

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectableType _type;
}
