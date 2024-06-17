using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorUI : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeTime = 1f;

    private Transform player;
    private Vector3 damageLocation;
    private float fadeTimer;
    private bool isReleased;

    public void SetUp (Transform player, Vector3 damageLocation)
    {
        this.player = player;
        this.damageLocation = damageLocation;
        fadeTimer = fadeDelay;
        canvasGroup.alpha = 1;
        isReleased = false;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
        }
        else
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            if (!isReleased && canvasGroup.alpha <= 0)
            {
                isReleased = true;
                ObjectPoolManager.ReleaseObject(this);
            }
        }

        damageLocation.y = player.position.y;
        Vector3 dir = (damageLocation - player.position).normalized;
        float angle  = (Vector3.SignedAngle(dir, player.forward, Vector3.up));
        pivot.localEulerAngles = new Vector3(0, 0, angle);
    }   
}
