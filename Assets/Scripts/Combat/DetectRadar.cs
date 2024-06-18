using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRadar : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float scanSize;
    [SerializeField] int maxScanAtOnce;
    [SerializeField] LayerMask scanLayer;
    [SerializeField] Color scanColor = Color.red;

    [Header("Sight Properties")]
    [SerializeField] LayerMask obstacleLayer;
    public float angle = 30;

    [Header("Mesh")]
    [SerializeField] float height = 1.0f;

    [Header("Checks")]
    [SerializeField] bool storeTargets = false;
    [SerializeField] bool searchForTarget = false;
    [SerializeField] bool checkIfTargetInSite = false;

    [Space]

    [Header("Debug")]
    [SerializeField] bool targetInRange = false;
    [SerializeField] bool targetInSite = false;

    [SerializeField] List<GameObject> objectsInRange = new List<GameObject>();

    int count;
    Collider[] colliders;

    void Start()
    {
        colliders = new Collider[maxScanAtOnce];
    }

    void Update()
    {
        if (!searchForTarget) return;
        Scan();
    }

    public GameObject ClosesetTargetInRange() => transform.FindClosestObject(objectsInRange);
    public GameObject TargetObjectInRange() => colliders[0].gameObject;

    public void ClearTargetList() => objectsInRange.Clear();
    public void SetUp(int size) { scanSize = size; }
    public float GetScanSize() => scanSize; 
    public void ToggleSearch(bool newState) { searchForTarget = newState; }
    public bool TargetInRange() { return targetInRange; }
    public bool TargetInSight() { return targetInSite; }

    public void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, scanSize, colliders, scanLayer, QueryTriggerInteraction.Collide);

        targetInRange = count > 0;

        targetInSite = (checkIfTargetInSite && targetInRange) ? IsInSight(colliders[0]) : false;

        if (storeTargets) StoreTargets();
    }

    bool IsInSight(Collider targetCol)
    {
        Vector3 directionToTarget = Vector3.Normalize(targetCol.bounds.center - transform.position);
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if(angleToTarget < angle)
        {
            if(!Physics.Linecast(transform.position, targetCol.bounds.center, out RaycastHit hit, obstacleLayer))
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

    void StoreTargets()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            objectsInRange.Add(obj);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = scanColor;
        Gizmos.DrawWireSphere(transform.position, scanSize);

        Vector3 rightDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * scanSize);

        Vector3 leftDirection = Quaternion.Euler(0, -angle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDirection * scanSize);
    }
}
