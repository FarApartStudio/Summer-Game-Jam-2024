using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "Stat Management/ New Stat")]
public class StatSO : ScriptableObject
{
    [SerializeField] private CustomAnimationCurve statCurve;
    public CustomAnimationCurve StatCurve => statCurve;

    private void OnValidate()
    {
        if (statCurve.curveType != CurveType.Custom)
        {
            statCurve.UpdateCurve();
        }
    }

    public int GetCurrentValue(int level) => statCurve.GetCurrentValueInt(level);
}

