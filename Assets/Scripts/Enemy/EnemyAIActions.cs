using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public static class EnemyAIActions 
{
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

    public static T GetFurthest<T>(this Transform owner, List<T> others) where T : Component
    {
        T furthestPlayerPos = null;
        float distance = 0;
        Vector3 position = owner.position;

        foreach (T otherPos in others)
        {
            Vector3 diff = otherPos.transform.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance > distance)
            {
                furthestPlayerPos = otherPos;
                distance = curDistance;
            }
        }
        return furthestPlayerPos;
    }

    public static void LookAtTarget(this Transform ownerTranfrom, Vector3 targetPosition)
    {
        Vector3 tragetPosition = new Vector3(targetPosition.x, ownerTranfrom.position.y, targetPosition.z);
        ownerTranfrom.LookAt(tragetPosition);
    }

    public static void LookAtTargetSmooth(this Transform ownerTranfrom, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - ownerTranfrom.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        ownerTranfrom.rotation = Quaternion.Slerp(ownerTranfrom.rotation, lookRotation, Time.deltaTime * 5);
    }

    public static void LookAtTargetSmooth(this Transform ownerTranfrom, Transform targetPosition)
    {
        Vector3 direction = (targetPosition.position - ownerTranfrom.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        ownerTranfrom.rotation = Quaternion.Slerp(ownerTranfrom.rotation, lookRotation, Time.deltaTime * 5);
    }

    public static bool CheckDistanceToTarget (this Transform ownerPos, Transform targetPos, float range)
    {
        if (Vector3.Distance(ownerPos.position, targetPos.position) < range)
            return true;
        return false;
    }

   public static bool RandomPosition(Vector3 center, float range, out Vector3 result)
   {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        int loopCount = 0;

        do
        {
            randomPoint = center + Random.insideUnitSphere * range;
            loopCount++;
        } while (!NavMesh.SamplePosition(randomPoint, out hit, .5f, NavMesh.AllAreas) && loopCount < 30);

        if (loopCount >= 30)
        {
            result = center;
            return false;
        }

        result = hit.position;
        return true;
    }

    public static bool IsPositionValid(Vector3 position, out Vector3 result)
    {
        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(position, out hit, .5f, NavMesh.AllAreas);
        result = hit.position;
        return isValid;
    }

    public static void TeleportToPosition(NavMeshAgent navAgent,  Vector3 newPos)
    {
        navAgent.Warp(newPos);
    }

    public static GameObject FindClosestObject(this Transform ownerPos, List<GameObject> gameObjects)
    {
        float distanceToClosestObject = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (GameObject currentObject in gameObjects)
        {
            float distanceToEnemy = (currentObject.transform.position - ownerPos.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestObject)
            {
                distanceToClosestObject = distanceToEnemy;
                closestObject = currentObject;
            }
        }

        return closestObject;
    }

    public static bool IsInSight(this Transform transform, Collider targetCol ,float angle, LayerMask obstacleLayer)
    {
        Vector3 directionToTarget = Vector3.Normalize(targetCol.bounds.center - transform.position);
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget < angle)
        {
            if (!Physics.Linecast(transform.position, targetCol.bounds.center, out RaycastHit hit, obstacleLayer))
            {
                Debug.DrawLine(transform.position, targetCol.bounds.center, Color.red);
                return true;
            }
            else
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }
        return false;
    }

    public static void FireProjectileAtPoint(this Transform enemyPosition, GameObject cannon, float angle,Vector3 targetPosition)
    {
        Vector3 velocity = BallisticVelocity(enemyPosition,targetPosition, angle);
        if (cannon != null)
        {
            //velocity.x = Mathf.Max(-0.01f, velocity.x);
            //velocity.y = Mathf.Max(-0.01f, velocity.y);
            //velocity.z = Mathf.Max(-0.01f, velocity.z);

            cannon.GetComponent<Rigidbody>().velocity = velocity;
        } 
    }

    public static Vector3 BallisticVelocity(Transform enemyPosition,Vector3 destination, float angle)
    {
        Vector3 dir = destination - enemyPosition.position; // get Target Direction
        float height = dir.y; // get height difference
        dir.y = 0; // retain only the horizontal difference
        float dist = dir.magnitude; // get horizontal direction
        float a = angle * Mathf.Deg2Rad; // Convert angle to radians
        dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle.
        dist += height / Mathf.Tan(a); // Correction for small height differences

        // Calculate the velocity magnitude
        float velocity = Mathf.Sqrt(Mathf.Max(0.01f, dist) * Physics.gravity.magnitude / Mathf.Sin(2 * a));

        return velocity * dir.normalized; // Return a normalized vector.
    }

    public static IEnumerator KnockBack(this NavMeshAgent navAgent, Vector3 direction,float knockBackDuration, float normalSpeed, float force = 20)
    {
        navAgent.speed = 10;
        navAgent.angularSpeed = 0;//Keeps the enemy facing forwad other than spinning
        navAgent.acceleration = 999;

        float currentDuration = knockBackDuration;

        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            navAgent.velocity = direction * force;//Knocks the enemy back when appropriate 

            yield return null;
        }

        //Reset to default values
        navAgent.speed = normalSpeed;
        navAgent.angularSpeed = 999;
        navAgent.acceleration = 999;
    }
}
