using UnityEngine;


[CreateAssetMenu(menuName = "Camera Shake/Profile", fileName = "Profile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse source settings")]
    public float shakeTime = 0.2f;
    public float shakeForce = 1f;
    public Vector3 defaultvelocity = new Vector3(0, -1f, 0);
    public AnimationCurve impulseCurve;

    [Header("Impulse listener settings")]
    public float listenerAmplitude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;
}