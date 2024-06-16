using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorUI : MonoBehaviour
{
    [SerializeField] private Vector3 damageLocation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform pivot;

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        damageLocation.y = player.position.y;
        Vector3 dir = (damageLocation - player.position).normalized;
        float angle  = (Vector3.SignedAngle(dir, player.forward, Vector3.up));
        pivot.localEulerAngles = new Vector3(0, 0, angle);
    }   
}
