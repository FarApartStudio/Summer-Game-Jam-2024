using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuicyShakeOnTrigger : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakePower = 0.5f;
    [SerializeField] private int vibrateCount = 10;
    [SerializeField] private bool ignoreShakeIfRunning;

    private Vector3 lastPos;
    CoroutineHandle shakeRoutine;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = (other.transform.position - transform.position).normalized;
        direction.y = 0;

        if (ignoreShakeIfRunning)
        {
            if (shakeRoutine == null || shakeRoutine.IsDone)
            {
                shakeRoutine = transform.JuicyShakePosition(shakeDuration, direction * shakePower, vibrateCount);
            }
        }
        else
        {
            if (shakeRoutine != null && !shakeRoutine.IsDone)
            {
                transform.position = lastPos;
                Juicer.StopCoroutine(shakeRoutine);
            }
            lastPos = transform.position;
            shakeRoutine = transform.JuicyShakePosition(shakeDuration, direction * shakePower, vibrateCount);
        }
    }
}
