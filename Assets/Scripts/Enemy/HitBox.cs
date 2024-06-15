using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitDirection { None, Front, Back, Left, Right }

public interface IDamageable
{
    void Damage(DamageInfo damageInfo);

    HitDirection GetHitDirection(DamageInfo damageInfo);


}

public class HitBox : MonoBehaviour,IDamageable
{
    public string bodyPart;

    public int damageMultiplier;

    public delegate void OnHitEventHandler (DamageInfo damageInfo);
    public event OnHitEventHandler OnHit;


    public void Damage(DamageInfo damageInfo)
    {
        damageInfo.damage *= damageMultiplier;
       // damageInfo.HitDirection = GetHitDirection(damageInfo);
        OnHit?.Invoke(damageInfo);
    }

    public HitDirection GetHitDirection(DamageInfo damageInfo)
    {
        HitDirection direction = HitDirection.None;

        //Vector3 localPoint = damageInfo.raycastHit.collider.transform.InverseTransformPoint(damageInfo.raycastHit.point);
        //Vector3 localDir = localPoint.normalized;

        //float fwdDot = Vector3.Dot(localDir, Vector3.forward);
        //float rightDot = Vector3.Dot(localDir, Vector3.right);

        //float fwdPower = Mathf.Abs(fwdDot);
        //float rightPower = Mathf.Abs(rightDot);

        //if (fwdPower > rightPower)
        //{
        //    if (fwdDot > 0) direction = HitDirection.Front;
        //    else direction = HitDirection.Back;

        //}
        //else
        //{
        //    if (rightDot > 0) direction = HitDirection.Right;
        //    else direction = HitDirection.Left;
        //}

        return direction;
    }
}
