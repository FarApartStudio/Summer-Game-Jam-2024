using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatfrom : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float speed;
    [SerializeField] Vector3[] wayPoints;

    [Header("Settings")]
    [SerializeField] bool canMove = false;
    [SerializeField] bool movePlayer;
    [SerializeField] bool loop;

    private Vector3 startPos;
    private Vector3 currentWayPoint;
    private int wayPointIndex = -1;
    private bool reverse;
    private Rigidbody rd;


    private void Awake()
    {
        rd = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startPos = transform.localPosition;
        currentWayPoint = GetCorrectLocalPostion(wayPoints[0]);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            canMove = !canMove;
        }


        if (!canMove) return;
        MoveToWayPoint();
        CheckDistanceToWayPoint();
    }

    public void MoveToWayPoint()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentWayPoint, speed * Time.deltaTime);
    }

    public void CheckDistanceToWayPoint()
    {
        float distance = Vector3.Distance(transform.localPosition, currentWayPoint);

        if (distance <= 0.2f)
        {
            if (loop)
            {
                if (wayPointIndex == wayPoints.Length - 1)
                {
                    wayPointIndex = -1;
                    currentWayPoint = startPos;
                }
                else
                {
                    wayPointIndex++;
                    currentWayPoint = GetCorrectLocalPostion(wayPoints[wayPointIndex]);
                }
            }
            else
            {
                if (wayPointIndex == 0)
                {
                    if (reverse)
                    {
                        wayPointIndex = -1;
                        currentWayPoint = startPos;
                    }
                    else
                    {
                        wayPointIndex++;
                        currentWayPoint = GetCorrectLocalPostion(wayPoints[wayPointIndex]);
                    }

                    reverse = false;
                }
                else
                {
                    if (wayPointIndex == wayPoints.Length - 1)
                    {
                        reverse = true;
                        wayPointIndex--;
                    }
                    else
                    {
                        if (reverse)
                        {
                            wayPointIndex--;
                        }
                        else
                        {
                            wayPointIndex++;
                        }
                    }

                    currentWayPoint = GetCorrectLocalPostion(wayPoints[wayPointIndex]);
                }
            }
        }
    }


    public Vector3 GetCorrectLocalPostion(Vector3 point)
    {
        return  point + startPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (movePlayer && other.TryGetComponent(out Pilot player))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (movePlayer && other.TryGetComponent(out Pilot player))
        {
            other.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // only in editor mode

#if UNITY_EDITOR

        if (!EditorApplication.isPlaying) startPos = transform.localPosition;

#endif
        Gizmos.DrawWireSphere(startPos, .2f);
        Gizmos.DrawLine(startPos, GetCorrectLocalPostion(wayPoints[0]));

        for (int i = 0; i < wayPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(GetCorrectLocalPostion(wayPoints[i]), .2f);
        }

        for (int i = 0; i < wayPoints.Length; i++)
        {
            Gizmos.DrawLine(GetCorrectLocalPostion(wayPoints[i]), GetCorrectLocalPostion(wayPoints[(i + 1) % wayPoints.Length]));
        }

        if (loop) Gizmos.DrawLine(startPos, GetCorrectLocalPostion(wayPoints[wayPoints.Length - 1]));
    }
}