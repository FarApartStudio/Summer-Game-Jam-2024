using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] DamageDefector damageDefector;
    [SerializeField] DetectRadar searchDetectRadar;
    [SerializeField] DetectRadar damageDetectRadar;

    [SerializeField] Animator animator;

    [Header("Hit Points")]
    [SerializeField] HitPoint[] hitPoints;

    private void Awake()
    {
        damageDefector.OnDefect += OnDefect;
    }

    private void OnDefect()
    {
        animator.CrossFade("Defend", 0.1f);
    }

    public IEnumerator GoToTarget(Transform playerTransfrom, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = playerTransfrom.position;
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            playerTransfrom.position = Vector3.Lerp(startPosition, targetPosition, timer / duration);
        }
    }



    protected bool IsWithinAngle(Vector3 direction, Vector3 forwardTransfrom, float angle)
    {
        return Vector3.Angle(direction, forwardTransfrom) < angle;
    }
}
