using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Camera Shake/Trigger", fileName = "Trigger")]
public class ScreenShakeTriggerSO : ScriptableObject
{
    public Cinemachine.CinemachineImpulseDefinition.ImpulseShapes shape;
    public float duration = 0.1f;
    public float force = 1f;
    public Vector2 velocity = default;
}
