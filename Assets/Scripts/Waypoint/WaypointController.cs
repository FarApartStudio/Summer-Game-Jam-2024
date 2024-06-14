using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<IWaypoint> waypoints = new List<IWaypoint>();

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateWaypoints();
    }

    public void SetPlayerTransform(Transform transform)
    {
        player = transform;
    }

    public void AddWaypoint(IWaypoint waypoint)
    {
        waypoints.Add(waypoint);
    }

    public void RemoveWaypoint(IWaypoint waypoint)
    {
        waypoints.Remove(waypoint);
    }

    public void UpdateWaypoints()
    {
        if (player == null)
        {
            return;
        }

        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].gameObject.SetActive(waypoints[i].WaypointInsigator.ShowWayPoint);

            if (waypoints[i].WaypointInsigator.ShowWayPoint)
            {
                float minX = waypoints[i].Img.GetPixelAdjustedRect().width / 1.5f;
                float maxX = Screen.width - minX;

                float minY = waypoints[i].Img.GetPixelAdjustedRect().width / 1.5f;
                float maxY = Screen.height - minY;

                Vector2 pos = mainCamera.WorldToScreenPoint(waypoints[i].WaypointInsigator.owner.position + waypoints[i].WaypointInsigator.Offset);
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minX, maxY);

                waypoints[i].Img.transform.position = pos;

                if (waypoints[i].MeterText != null)
                {
                    waypoints[i].MeterText.text = waypoints[i].WaypointInsigator.ShowWayDistance ?
                        ((int)Vector3.Distance(waypoints[i].WaypointInsigator.owner.position, player.position)).ToString() + "m" : "";
                }
            }
        }
    }
}
