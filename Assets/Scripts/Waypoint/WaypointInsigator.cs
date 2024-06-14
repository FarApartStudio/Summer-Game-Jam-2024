using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointInsigator : MonoBehaviour, IWaypointInsigator
{
    [SerializeField] private Sprite icon;
    [SerializeField] private bool showWayPoint;
    [SerializeField] private bool showWayDistance;
    [SerializeField] private Vector3 offset;

    public Sprite Icon => icon;
    public bool ShowWayPoint => showWayPoint;
    public bool ShowWayDistance => showWayDistance;
    public Vector3 Offset => offset;
    public Transform owner => transform;

    public void SetShowWayDistance(bool show)
    {
        showWayDistance = show;
    }

    public void SetShowWayPoint(bool show)
    {
        showWayPoint = show;
    }
}
