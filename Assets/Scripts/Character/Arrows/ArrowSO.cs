using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArrowSO", menuName = "ArrowSO")]
public class ArrowSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Arrow _arrowPrefab;

    public string GetName => _name;
    public Sprite GetIcon => _icon;
    public Arrow GetArrowPrefab => _arrowPrefab;
}
