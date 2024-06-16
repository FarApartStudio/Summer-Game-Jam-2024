using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransfrom : MonoBehaviour
{
    [SerializeField] private bool useForward;
    [SerializeField] Vector3 offset;
    [SerializeField] float speed;
    [SerializeField] private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void LateUpdate()
    {
        Follow();
    }


    public void Follow()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            if (useForward)
            {
                targetPos = target.position + target.forward * offset.z + target.right * offset.x + target.up * offset.y;
            }
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
    }
}
