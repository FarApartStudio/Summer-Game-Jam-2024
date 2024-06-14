using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITiltEffect : MonoBehaviour
{
    [Header("Tilt Effect")]
    [SerializeField] private bool shouldTilt = true;
    [SerializeField] private float tiltSpeed = 10f;
    [SerializeField] private float autoTiltAmount = 10f;

    private void Update()
    {
        if (shouldTilt)
        RotateCirclar();
    }

    private void RotateCirclar()
    {
        float sine = Mathf.Sin(Time.time);
        float cosine = Mathf.Cos(Time.time);
        float lerpX = Mathf.LerpAngle(transform.eulerAngles.x, sine * autoTiltAmount, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(transform.eulerAngles.y, cosine * autoTiltAmount, tiltSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(lerpX, lerpY, 0);
    }

    public void SetTilt(bool value)
    {
        shouldTilt = value;

        if (!shouldTilt)
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
}
