using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class GameUtility
{
    public static void FaceVelocityDirection(this Rigidbody rigidbody)
    {
        rigidbody.transform.localScale = new Vector2(rigidbody.velocity.x > 0 ? 1 : -1, 1);
    }

    public static void FaceVelocityDirection(this Transform transform, Rigidbody rigidbody, float scale)
    {
        if (rigidbody.velocity.x == 0) return;
        float scaleX = (rigidbody.velocity.x > 0) ? scale * 1 : scale * -1;
        transform.localScale = new Vector3(scaleX, scale, scale);
    }

    public static void LookAtTarget(this Transform ownerTranfrom, Vector3 targetPosition)
    {
        Vector3 targetPos = new Vector3(targetPosition.x, ownerTranfrom.position.y, targetPosition.z);
        ownerTranfrom.LookAt(targetPos);
    }

    public static void LookAtTargetSmooth(this Transform ownerTranfrom, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - ownerTranfrom.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        ownerTranfrom.rotation = Quaternion.Slerp(ownerTranfrom.rotation, lookRotation, Time.deltaTime * 5);
    }

    public static void LookAtTargetSmooth(this Transform ownerTranfrom, Transform targetPosition)
    {
        LookAtTargetSmooth (ownerTranfrom, targetPosition.position);
    }

    public static bool ObstacleDetectedRaycast(Transform startTransfrom, Vector2 normalisedDirection, float distanceCheck, LayerMask detectLayer, bool showDebug = true)
    {
        if (showDebug) Debug.DrawRay(startTransfrom.position, normalisedDirection * distanceCheck, Color.yellow);

        RaycastHit2D hit = Physics2D.Raycast(startTransfrom.position, normalisedDirection, distanceCheck, detectLayer);

        if (hit.collider != null)
        {
            if (showDebug) Debug.DrawLine(startTransfrom.position, hit.point, Color.red);
            return true;
        }
        else return false;
    }

    public static bool ObstacleDetectedBoxcast(Collider collider, float normalisedDirection, float distanceCheck, LayerMask detectLayer, bool showDebug = true)
    {
        if (showDebug) DrawBoxcast2D(collider.bounds.center, collider.bounds.size, normalisedDirection, distanceCheck, Color.yellow);

        RaycastHit2D hit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, new Vector2(normalisedDirection, 0), distanceCheck, detectLayer);

        if (hit.collider != null)
        {
            if (showDebug) DrawBoxcast2D(collider.bounds.center, collider.bounds.size, normalisedDirection, distanceCheck, Color.red);
            return true;
        }
        else return false;
    }

    public static bool GroundDetected(Transform transform, Vector2 groundOffset, float groundRange, LayerMask groundLayer, bool showDebug = true)
    {
        Vector2 currentOffset = new Vector2(groundOffset.x * -transform.localScale.x, groundOffset.y);
        Collider2D ground = Physics2D.OverlapCircle((Vector2)transform.position + currentOffset, groundRange, groundLayer);

        if (showDebug) Debug.DrawRay((Vector2)transform.position + currentOffset, Vector2.down * groundRange, Color.yellow);

        if (ground == null)
        {
            if (showDebug) Debug.DrawRay((Vector2)transform.position + currentOffset, Vector2.down * groundRange, Color.red);
            return false;
        }
        else return true;
    }

    public static bool TargetInRange(this Transform self, Transform target, float distance)
    {
        return Vector2.Distance(self.transform.position, target.position) <= distance;
    }

    public static bool TargetInRangOnX(this Transform self, Transform target, float distance)
    {
        return Mathf.Abs(self.transform.position.x - target.transform.position.x) <= distance;
    }

    public static float GetDistanceFromTarget(this Transform self, Vector2 target)
    {
        return Vector2.Distance(self.position, target);
    }

    public static float GetNormalisedDirection(Vector2 start, Vector2 target)
    {
        return -(start - target).normalized.x > 0 ? 1 : -1;
    }

    public static float GetDirectionToPosition(this Transform transform, Vector2 target)
    {
        return (target.x - transform.position.x) > 0 ? 1 : -1;
    }

    public static void MoveToPosition(Rigidbody rigidbody, Vector2 targetPostion, float moveSpeed, bool flying = false)
    {
        Vector2 positionToGo;

        if (flying)
        {
            positionToGo = Vector2.MoveTowards(rigidbody.position, targetPostion, moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Vector2 correctPosition = new Vector2(targetPostion.x, rigidbody.position.y);
            positionToGo = Vector2.MoveTowards(rigidbody.position, correctPosition, moveSpeed * Time.fixedDeltaTime);
        }

        rigidbody.MovePosition(positionToGo);
    }

    public static void MoveToDirection(Rigidbody rigidbody, float direction, float moveSpeed)
    {
        rigidbody.velocity = new Vector2(direction * moveSpeed, rigidbody.velocity.y);
    }

    public static void LookAtHorizontalDirection(this Transform transform, float direction, float scale)
    {
        if (direction == 0) return;
        float scaleX = (direction > 0) ? scale * 1 : scale * -1;
        transform.localScale = new Vector3(scaleX, scale, scale);
    }

    public static void LookAtTransfrom_ScaleFip(this Transform self, Transform target, float scale)
    {

        float direction = self.position.x - target.position.x;
        float scaleX = (Mathf.RoundToInt(direction) > 0) ? scale : -scale;
        self.localScale = new Vector3(scaleX, self.localScale.y, self.localScale.z);
    }

    public static void Jump(Rigidbody rigidbody, float jumpForce)
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
    }

    public static Vector2 GetMidPos(this Transform self, Vector2 point)
    {
        Vector2 newPos = new Vector2(point.x, point.y);
        newPos.x -= self.GetComponent<Collider2D>().bounds.extents.x;
        return newPos;
    }

    public static IEnumerator KnockBack(Rigidbody rigidbody, float knockDownDuration, float knockDownPower, Transform obj)
    {
        float timer = 0;

        while (knockDownDuration > timer)
        {
            timer += Time.deltaTime;
            Vector2 direction = (obj.position - rigidbody.transform.position).normalized;
            rigidbody.AddForce(-direction * knockDownPower, ForceMode.Impulse);
        }
        yield return null;
    }

    public static IEnumerator KnockBack(this Rigidbody rigidbody, Vector2 direction, float force, float duration, Action OnBegin = null, Action OnEnd = null)
    {
        OnBegin?.Invoke();
        rigidbody.AddForce(direction * force, ForceMode.Impulse);
        yield return new WaitForSeconds(duration);
        rigidbody.velocity = Vector2.zero;
        OnEnd?.Invoke();
    }

    public static IEnumerator AddVelocity(this Rigidbody rigidbody, Vector2 direction, float force, float duration, Action OnBegin = null, Action OnEnd = null)
    {
        OnBegin?.Invoke();

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            rigidbody.AddForce(direction * force, ForceMode.Impulse);
            yield return null;
        }
        OnEnd?.Invoke();
    }

    public static int ScaleToDirection(float scale)
    {
        return scale > 0 ? 1 : -1;
    }

    public static T GetClosest<T>(this Transform owner, List<T> others) where T : Component
    {
        T closestPlayerPos = null;
        float distance = Mathf.Infinity;
        Vector3 position = owner.position;

        foreach (T otherPos in others)
        {
            Vector3 diff = otherPos.transform.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closestPlayerPos = otherPos;
                distance = curDistance;
            }
        }
        return closestPlayerPos;
    }

    public static Transform GetClosestPlayer(this Transform owner, List<Transform> others)
    {
        Transform closestPlayerPos = null;
        float distance = Mathf.Infinity;
        Vector3 position = owner.position;

        foreach (Transform otherPos in others)
        {
            Vector3 diff = otherPos.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closestPlayerPos = otherPos;
                distance = curDistance;
            }
        }
        return closestPlayerPos;
    }

    public static Transform GetFurthestPlayer(this Transform owner, List<Transform> others)
    {
        Transform furthestPlayerPos = null;
        float distance = 0;
        Vector3 position = owner.position;

        foreach (Transform otherPos in others)
        {
            Vector3 diff = otherPos.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance > distance)
            {
                furthestPlayerPos = otherPos;
                distance = curDistance;
            }
        }
        return furthestPlayerPos;
    }

    public static void ShowDamagePop(Transform spawner, GameObject objectToSpawn, Vector3 offset, Vector3 randomIntensity, float amount)
    {
        GameObject enemyDamgePop = GameObject.Instantiate(objectToSpawn, spawner.position, spawner.rotation);
        enemyDamgePop.transform.position = spawner.position + offset;
        enemyDamgePop.transform.localPosition += new Vector3(
            Random.Range(-randomIntensity.x, randomIntensity.x),
            Random.Range(-randomIntensity.x, randomIntensity.y),
            Random.Range(-randomIntensity.x, randomIntensity.z));
        enemyDamgePop.GetComponent<TextMeshPro>().text = "-" + amount.ToString();
        enemyDamgePop.GetComponent<TextMeshPro>().color = Color.red;
    }

    public static bool HasLayer(this Component component, LayerMask layerMask)
    {
        return (layerMask.value & 1 << component.gameObject.layer) > 0;
    }

    public static Vector2 GetChildPosition(this ScrollRect scrollRect, RectTransform target, bool isCenter = true)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 targetLocalPosition = target.localPosition;
        float x = scrollRect.vertical ? scrollRect.content.localPosition.x :
            0 - (viewportLocalPosition.x + targetLocalPosition.x) + (!isCenter && scrollRect.horizontal ? (scrollRect.viewport.rect.width / 2 - target.rect.width / 2) : 0);
        float y = scrollRect.horizontal ? scrollRect.content.localPosition.y :
            0 - (viewportLocalPosition.y + targetLocalPosition.y) + (!isCenter && scrollRect.vertical ? (scrollRect.viewport.rect.height / 2 - target.rect.height / 2) : 0);
        Vector2 newTargetLocalPosition = new Vector2(x, y);
        return newTargetLocalPosition;
    }

    public static void DrawBoxcast2D(Vector2 position, Vector2 size, float direction, float distance, Color color)
    {
        Vector2 directionVector = new Vector2(direction, 0);
        Vector2 origin = position + (directionVector * size.x * 0.5f);
        Vector2 sizeVector = new Vector2(size.x, size.y);
        Debug.DrawRay(origin, directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.up * sizeVector.y * 0.5f), directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.down * sizeVector.y * 0.5f), directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.up * sizeVector.y * 0.5f) + (directionVector * distance), Vector2.down * sizeVector.y, color);
        Debug.DrawRay(origin + (Vector2.down * sizeVector.y * 0.5f) + (directionVector * distance), Vector2.up * sizeVector.y, color);
    }

    public static void DebugDrawBox(Vector2 center, Vector2 size, Color color)
    {
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;
        Debug.DrawLine(new Vector3(center.x + halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x - halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y + halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y - halfHeight, 0), color);
    }
}
