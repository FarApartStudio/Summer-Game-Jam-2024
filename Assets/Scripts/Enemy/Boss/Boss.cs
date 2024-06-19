using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private DamageDefector damageDefector;
    [SerializeField] private DetectRadar searchDetectRadar;
    [SerializeField] private DetectRadar damageDetectRadar;
    [SerializeField] private Transform target;

    [Header("Properties")]
    [SerializeField] float gapToTarget = 1.5f;
    [SerializeField] float waitBeforeRetreat = 1.5f;

    [SerializeField] Animator animator;

    [Header("Hit Points")]
    [SerializeField] HitPoint[] hitPoints;

    private Vector3 startPos;

    public Transform GetTarget => target;

    private void Awake()
    {
        damageDefector.OnDefect += OnDefect;
    }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BiteAttack());
        }
    }

    private void OnDefect()
    {
        animator.CrossFade("Defend", 0.1f);
    }

    public IEnumerator BiteAttack()
    {
        animator.CrossFade("BiteAttack", 0.1f);

        Vector3 firstDestination = new Vector3(target.position.x, startPos.y, target.position.z - gapToTarget);
        yield return GoToTarget (transform, firstDestination, 1.5f);
        yield return new WaitForSeconds(waitBeforeRetreat);
        yield return GoToTarget(transform, startPos, 1.5f);
        animator.CrossFade("Idle", 0.1f);
    }

    public void GoToTarget(Vector3 targetPosition, float duration, Action OnComplete = null)
    {
        StartCoroutine(GoToTarget(transform, targetPosition, duration, OnComplete));
    }

    public IEnumerator GoToTarget(Transform playerTransfrom, Vector3 targetPosition, float duration, Action OnComplete = null)
    {
        Vector3 startPosition = playerTransfrom.position;
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            playerTransfrom.position = Vector3.Lerp(startPosition, targetPosition, timer / duration);
        }

        OnComplete?.Invoke();
    }


    protected bool IsWithinAngle(Vector3 direction, Vector3 forwardTransfrom, float angle)
    {
        return Vector3.Angle(direction, forwardTransfrom) < angle;
    }
}
