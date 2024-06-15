using Pelumi.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WayPointMenu : GenericMenu<WayPointMenu>
{
    [SerializeField] private WaypointUI waypointPrefab;
    [SerializeField] private Transform waypointParent;
    [SerializeField] private WaypointController waypointController;

    private List<WaypointUI> waypoints = new List<WaypointUI>();

    protected override void OnCreated()
    {
 
    }

    protected override void OnOpened()
    {

    }

    protected override void OnClosed()
    {

    }

    protected override void OnDestoryInvoked()
    {

    }

    public override void ResetMenu()
    {

    }

    public void AddWaypoint(IWaypointInsigator waypointInsigator)
    {
        WaypointUI waypoint = Instantiate(waypointPrefab, waypointParent);
        waypoint.SetUp(waypointInsigator);
        waypoints.Add(waypoint);
        waypointController.AddWaypoint(waypoint);
    }

    public void RemoveWaypoint(IWaypointInsigator waypointInsigator)
    {
        WaypointUI waypoint = waypoints.Find(x => x.WaypointInsigator == waypointInsigator);
        waypointController.RemoveWaypoint(waypoint);
        waypoints.Remove(waypoint);
        Destroy(waypoint.gameObject);
    }

    public void SetPlayerTransform(Transform transform)
    {
        waypointController.SetPlayerTransform(transform);
    }
}
