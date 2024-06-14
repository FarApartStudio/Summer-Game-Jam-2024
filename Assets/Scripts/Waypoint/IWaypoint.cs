using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IWaypoint
{
    public GameObject gameObject { get; }
    public Image Img { get; }
    public TextMeshProUGUI MeterText { get; }

    public IWaypointInsigator WaypointInsigator { get; }

    void SetUp(IWaypointInsigator waypointInsigator);
}


public interface IWaypointInsigator
{
    public Sprite Icon { get; }
    public bool ShowWayPoint { get; }
    public bool ShowWayDistance { get; }
    public Vector3 Offset { get; }
    public Transform owner { get; }

    void SetShowWayPoint(bool show);
    void SetShowWayDistance(bool show);
}